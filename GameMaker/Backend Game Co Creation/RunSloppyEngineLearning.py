import numpy as np 
from state import *
from engine import *
import random
import pickle
import operator
import csv
import glob
from imageio import imwrite
from skimage.transform import resize
from heapq import * 

#2:30 for refine 11 : 44 rules
#30 for sloppy 11 : 41 rules
#sloppy into refine still broken

def DrawFrame(frame, frameName):
	imwrite(frameName, frame)

#Check distance between two frame images
def FrameImageDistance(frameImage1, frameImage2):
	dist = 0.0
	badPixels = []
	for x in range(0, 105):
		for y in range(0, 80):
			if not tuple(frameImage1[x][y])==tuple(frameImage2[x][y]):
				badPixels.append([frameImage1[x][y], frameImage2[x][y], x, y])
				dist+=1.0
	dist/=(105.0*80.0)
	return dist

def PredictedStateDistance(state1, state2, printIt=False):
	prePerfectMatches = []
	postPerfectMatches = []
	
	for cid in range(0, len(state1.components)):
		for cid2 in range(0, len(state2.components)): 
			if ComponentToComponentHellmansMetric(state1.components[cid], state2.components[cid2])==0:
				if not cid in prePerfectMatches and not cid2 in postPerfectMatches:
					#Ensure it has all facts matched
					if len(state1.factsByComponentID[cid])==len(state2.factsByComponentID[cid2]):
						internalMatched2 = []
						allmatched = True
						for fact in state1.factsByComponentID[cid]:
							matched = False
							for fact2 in state2.factsByComponentID[cid2]:
								if not fact2 in internalMatched2:
									if fact.CheckMatchBesidesID(fact2):
										internalMatched2.append(fact2)
										matched = True
										break
							if not matched: 
								allmatched = False
								break
						if allmatched:
							prePerfectMatches.append(cid)
							postPerfectMatches.append(cid2)
	allPerfectComponentMatches = False
	if len(prePerfectMatches)==len(state1.components) and len(postPerfectMatches)==len(state2.components):
		allPerfectComponentMatches = True


	preUnmatched = state1.GetAllFacts()
	toRemove = []
	postToMatch = []
	for fact2 in state2.GetAllFacts():
		if not fact2.componentID in postPerfectMatches and not isinstance(fact2, RelationshipFactX) and not isinstance(fact2, RelationshipFactY):
			postToMatch.append(fact2)

	for preEffect in preUnmatched:
		#print ("Attempting to match pre fact "+str(preEffect))
		if preEffect.componentID in prePerfectMatches:
			#print ("Pre Rejected due to match of ID: "+str(preEffect.componentID))

			toRemove.append(preEffect)
		elif not isinstance(preEffect, RelationshipFactX) and not isinstance(preEffect, RelationshipFactY):
			postFactToRemove = None
			for fact2 in postToMatch:
				if preEffect.CheckMatchBesidesID(fact2):
					#print ("Pre Rejected due to match to fact: "+str(fact2))
					postFactToRemove = fact2
					break
			if not postFactToRemove==None:
				postToMatch.remove(postFactToRemove)
				toRemove.append(preEffect)
		else:
			#print ("Pre Rejected due to else")
			toRemove.append(preEffect)

	#Remove actions
	for fact in preUnmatched:
		if isinstance(fact, VariableFact) and fact.variableName=="action":
			if not fact in toRemove:
				toRemove.append(fact)
				break

	for r in toRemove:
		preUnmatched.remove(r)		

	postUnmatched = state2.GetAllFacts()
	toRemove = []
	preToMatch = []
	for fact in state1.GetAllFacts():
		if not fact.componentID in prePerfectMatches and not isinstance(fact, RelationshipFactX) and not isinstance(fact, RelationshipFactY):
			preToMatch.append(fact)

	for postEffect in postUnmatched:
		#print ("Attempting to match post fact "+str(postEffect))
		if postEffect.componentID in postPerfectMatches:
			#print ("Post Rejected due to match of ID: "+str(postEffect.componentID))
			toRemove.append(postEffect)
		elif not isinstance(postEffect, RelationshipFactX) and not isinstance(postEffect, RelationshipFactY):
			preFactToRemove = None
			for fact in preToMatch:
				if postEffect.CheckMatchBesidesID(fact):
					#print ("Post Rejected due to match to fact: "+str(fact))
					preFactToRemove = fact
					break
			if not preFactToRemove==None:
				preToMatch.remove(preFactToRemove)
				toRemove.append(postEffect)
		else:
			#print ("Post Rejected due to else")
			toRemove.append(postEffect)
	#Remove actions
	for fact in postUnmatched:
		if isinstance(fact, VariableFact) and fact.variableName=="action":
			if not fact in toRemove:
				toRemove.append(fact)
				break

	for r in toRemove:
		postUnmatched.remove(r)
	if printIt:		
		for preEffect in preUnmatched:
			print (".   PreEffect Unmatched: "+str(preEffect))
		#print ("")
		#for fact in state1.GetAllFacts():
		#	print (". Real True Pre Fact: "+str(fact))
		print ("")
		for postEffect in postUnmatched:
			print (".   PostEffect Unmatched: "+str(postEffect))
		#print ("")
		#for fact in state2.GetAllFacts():
		#	print (". Real True Post Fact: "+str(fact))
	
	distToReturn = len(preUnmatched)+len(postUnmatched)

	if not allPerfectComponentMatches:
		distToReturn+=1

	return distToReturn

'''
This class represents the initial components (e.g. sprites or game objects)
found during the initial processing step. 

It has a function ConvertToReal to re-represent it as a numpy matrix

'''
class ProcessingComponent:
	def __init__(self):
		self.myPixels = []
		self.xMin = float("inf")
		self.yMin = float("inf")
		self.xMax = 0
		self.yMax = 0

	#Add a pixel
	def AddPixel(self,x,y, pixel):
		if x<self.xMin:
			self.xMin = x
		if x>self.xMax:
			self.xMax = x

		if y<self.yMin:
			self.yMin = y
		if y>self.yMax:
			self.yMax = y

		self.myPixels.append((x,y,pixel))

	#Place the component back on the origin
	def ToZero(self):
		newPixels = []

		for p in self.myPixels:
			newX = p[0]-self.xMin
			newY = p[1]-self.yMin
			newPixels.append((newX,newY, p[2]))

		self.myPixels = newPixels

	#Re-represent the component as a numpy matrix
	#WARNING: ToZero MUST be called first
	def ConvertToReal(self):
		matrix = np.zeros((1+(self.xMax-self.xMin), 1+(self.yMax-self.yMin),3))#Assumes rgb
		for p in self.myPixels:
			matrix[p[0]][p[1]]= p[2]
		
		return matrix

	def HasEmptyPixels(self):
		area = (1+self.xMax-self.xMin)*(1+self.yMax-self.yMin)
		return area>len(self.myPixels)

'''
Recursively find components

observation - the current state, assumed to be 105x80x3
bgColor - the assumed background colorDictionary
currComponent - the current component
currX - the current x position in observation, must be a valid position
currY - the current y position in observation, must be a valid position

'''
#Add all possible neighbor engines based on modifying existing rules (1)
def GenerateNeighborEngineModifyRules(engine, closedEngineList, nextPredictedState,trueNextState,trueCurrState, openEngineHeapQ, preUnmatched, postUnmatched, neighbors):
	for r in range(0, len(engine.rules)):
		#Determine if rule fired
		fired, effectIds = engine.rules[r].ConditionSatisfiedCheck(trueCurrState)

		#Would it be helpful if this rule had fired in this instance
		if not fired:
			print ("MODIFY RULE HIT NOT FIRED")
			postEffectMatch = None
			helpfulIfHadFired = False
			for fact in postUnmatched: 
				if engine.rules[r].postEffect.CheckMatchBesidesID(fact):
					helpfulIfHadFired = True
					postEffectMatch = fact
					break

			if helpfulIfHadFired:
				print ("MODIFY RULE HIT HELPFUL HAD FIRED")
				#Find new set of conditions that would have led this rule to fire
				newConditions = []
				preEffect = engine.rules[r].preEffect.clone()
				for cond in engine.rules[r].conditions:
					matched = False

					for fact in trueCurrState.GetAllFacts():
						if cond.CheckMatchBesidesID(fact):
							matched = True
							newConditions.append(cond)
							break
					if not matched and not isinstance(cond, AnimationFact):
						#check if either of the two inequalities of this condition match
						inequalities = [InequalityFact(cond, cond.GetValue(), ">="), InequalityFact(cond, cond.GetValue(), "<=")]
						for ineqaulityCond in inequalities:
							for fact in trueCurrState.GetAllFacts():
								if ineqaulityCond.CheckMatchBesidesID(fact):
									matched = True
									newConditions.append(ineqaulityCond)
									if cond==preEffect:
										preEffect = ineqaulityCond
									break
							if matched:
								break
				print ("MODIFY RULE HIT NEW CONDITIONS: "+str(len(newConditions)))
				if len(newConditions)>0:#TODO; threshold parameter?
					clonedEngine = engine.clone()
					clonedEngine.rules[r] = Rule(newConditions, preEffect, clonedEngine.rules[r].postEffect)
					if not clonedEngine in closedEngineList and not clonedEngine in neighbors:
						neighbors.append(clonedEngine)
		#else: TODO; if it did fire and that made things worse, add hidden variable
	return neighbors

#Add all possible neighbor engines based on adding rules (2)
def GenerateNeighborEngineAddedRules(engine, closedEngineList, nextPredictedState,trueNextState,trueCurrState, openEngineHeapQ, preUnmatched, postUnmatched, neighbors):
	allFactsList = nextPredictedState.GetAllFacts()

	for preFact in preUnmatched:
		for postFact in postUnmatched:
			if isinstance(preFact, postFact.__class__):#If classes match
				clonedEngine = engine.clone()
				if not isinstance(preFact, PositionXFact) and not isinstance(preFact, PositionYFact):		
					clonedEngine.addRule(Rule(allFactsList, preFact, postFact))
					if not clonedEngine in closedEngineList and not clonedEngine in neighbors:
						neighbors.append(clonedEngine)

	return neighbors

#Add all possible neighbors based on deleting existing rules (3)
def GenerateNeighborEngineDeletingRules(engine, closedEngineList, nextPredictedState,trueNextState,trueCurrState, openEngineHeapQ, neighbors):
	print ("")
	print ("Deleting rules")
	#Delete rules
	if len(engine.rules)>1:
		for r in range(0, len(engine.rules)):			
			fired, effectIds = engine.rules[r].ConditionSatisfiedCheck(trueCurrState)
			#Remove it if it fired
			if fired: 
				clonedEngine = engine.clone()
				clonedEngine.rules.remove(clonedEngine.rules[r])
				if not clonedEngine in closedEngineList and not clonedEngine in neighbors:
					neighbors.append(clonedEngine)
	return neighbors

def GenerateNeighborEngines(engine, closedEngineList, nextPredictedState,trueNextState,trueCurrState, openEngineHeapQ):
	#Construct a greedy matching of pre to post perfect componentID matches
	prePerfectMatches = []
	postPerfectMatches = []
	preComponentNamesDict = {}
	postComponentNamesDict = {}

	firstIter = True
	for cid in range(0, len(nextPredictedState.components)):
		if not nextPredictedState.components[cid][0] in preComponentNamesDict.keys():
			preComponentNamesDict[nextPredictedState.components[cid][0]] = 0
		preComponentNamesDict[nextPredictedState.components[cid][0]] +=1

		for cid2 in range(0, len(trueNextState.components)): 
			if firstIter:
				if not trueNextState.components[cid2][0] in postComponentNamesDict.keys():
					postComponentNamesDict[trueNextState.components[cid2][0]] = 0
				postComponentNamesDict[trueNextState.components[cid2][0]] +=1

			if ComponentToComponentHellmansMetric(nextPredictedState.components[cid], trueNextState.components[cid2])==0:
				if not cid in prePerfectMatches and not cid2 in postPerfectMatches:
					#Ensure it has all facts matched
					if len(nextPredictedState.factsByComponentID[cid])==len(trueNextState.factsByComponentID[cid2]):
						internalMatched2 = []
						allmatched = True
						for fact in nextPredictedState.factsByComponentID[cid]:
							matched = False
							for fact2 in trueNextState.factsByComponentID[cid2]:
								if not fact2 in internalMatched2:
									if fact.CheckMatchBesidesID(fact2):
										internalMatched2.append(fact2)
										matched = True
										break
							if not matched: 
								allmatched = False
								break
						if allmatched:
							prePerfectMatches.append(cid)
							postPerfectMatches.append(cid2)
		firstIter = False
	#Given the perfect matches, find list of remaining unmatched pre and post facts
	preUnmatched = nextPredictedState.GetAllFacts()
	toRemove = []
	postToMatch = []
	for fact2 in trueNextState.GetAllFacts():
		if not fact2.componentID in postPerfectMatches and not isinstance(fact2, RelationshipFactX) and not isinstance(fact2, RelationshipFactY):
			postToMatch.append(fact2)

	for preEffect in preUnmatched:
		if preEffect.componentID in prePerfectMatches:
			toRemove.append(preEffect)
		elif not isinstance(preEffect, RelationshipFactX) and not isinstance(preEffect, RelationshipFactY):
			postFactToRemove = None
			for fact2 in postToMatch:
				if preEffect.CheckMatchBesidesID(fact2):
					postFactToRemove = fact2
					break
			if not postFactToRemove==None:
				postToMatch.remove(postFactToRemove)
				toRemove.append(preEffect)
		else:
			toRemove.append(preEffect)

	for r in toRemove:
		preUnmatched.remove(r)		
	print ("Preunmatched: "+str(len(preUnmatched)))

	postUnmatched = trueNextState.GetAllFacts()
	toRemove = []
	preToMatch = []
	for fact in nextPredictedState.GetAllFacts():
		if not fact.componentID in prePerfectMatches and not isinstance(fact, RelationshipFactX) and not isinstance(fact, RelationshipFactY):
			preToMatch.append(fact)

	for postEffect in postUnmatched:
		if postEffect.componentID in postPerfectMatches:
			toRemove.append(postEffect)
		elif not isinstance(postEffect, RelationshipFactX) and not isinstance(postEffect, RelationshipFactY):
			preFactToRemove = None
			for fact in preToMatch:
				if postEffect.CheckMatchBesidesID(fact):
					preFactToRemove = fact
					break
			if not preFactToRemove==None:
				preToMatch.remove(preFactToRemove)
				toRemove.append(postEffect)
		else:
			toRemove.append(postEffect)

	for r in toRemove:
		postUnmatched.remove(r)
			
	print ("Postunmatched: "+str(len(postUnmatched)))

	neighbors = []
	print ("Modifying Engine Rules")
	neighbors = GenerateNeighborEngineModifyRules(engine, closedEngineList, nextPredictedState,trueNextState,trueCurrState, openEngineHeapQ, preUnmatched, postUnmatched, neighbors)
	print ("Neighbors: "+str(len(neighbors)))
	print ("Adding Engine Rules")
	neighbors = GenerateNeighborEngineAddedRules(engine, closedEngineList, nextPredictedState,trueNextState,trueCurrState, openEngineHeapQ, preUnmatched, postUnmatched, neighbors)
	
	#Added rules
	#Disappear
	for cid in range(0, len(nextPredictedState.components)):
		if not cid in prePerfectMatches and ((not nextPredictedState.components[cid][0] in postComponentNamesDict.keys()) or (not preComponentNamesDict[nextPredictedState.components[cid][0]]==postComponentNamesDict[nextPredictedState.components[cid][0]])) and cid in nextPredictedState.factsByComponentID.keys():
			listAllFacts = []
			for fact in nextPredictedState.factsByComponentID[cid]:
				listAllFacts.append(fact)
			#remove all of them
			condition = nextPredictedState.GetAllFacts()
			#for c in condition:
			#	print ("ADDING CONDITION: "+str(c))

			clonedEngine = engine.clone()
			clonedEngine.addRule(Rule(condition, EmptyFact(listAllFacts), EmptyFact([])))

			if not clonedEngine in closedEngineList and not clonedEngine in neighbors:
				neighbors.append(clonedEngine)

	#Appear
	for cid in range(0, len(trueNextState.components)):
		if not cid in postPerfectMatches and ((not trueNextState.components[cid][0] in preComponentNamesDict.keys()) or (not preComponentNamesDict[trueNextState.components[cid][0]]==postComponentNamesDict[trueNextState.components[cid][0]])):
			minRelationshipFacts = trueNextState.GetMinRelationshipFactsToComponents(cid, nextPredictedState.components)

			condition = nextPredictedState.GetAllFacts()

			#add all of them after the relationship facts
			for fact in trueNextState.factsByComponentID[cid]:
				if not isinstance(fact, PositionXFact) and not isinstance(fact, PositionYFact):
					minRelationshipFacts.append(fact)

			clonedEngine = engine.clone()
			potentialRule = Rule(condition, EmptyFact([]), EmptyFact(minRelationshipFacts))
			if not potentialRule in clonedEngine.rules:
				clonedEngine.addRule(potentialRule)

				if not clonedEngine in closedEngineList and not clonedEngine in neighbors:
					neighbors.append(clonedEngine)
	#Transform
	for cid in range(0, len(nextPredictedState.components)):
		if not cid in prePerfectMatches:
			for cid2 in range(0, len(trueNextState.components)):
				if not cid2 in postPerfectMatches:
					c1Name = nextPredictedState.components[cid][0]
					c2Name = trueNextState.components[cid2][0]
					if (not c1Name in postComponentNamesDict.keys() or not preComponentNamesDict[c1Name]==postComponentNamesDict[c1Name]) and (not c2Name in preComponentNamesDict.keys() or not preComponentNamesDict[c2Name]==postComponentNamesDict[c2Name]):
						minRelationshipFacts = trueNextState.GetMinRelationshipFactsToComponents(cid2, nextPredictedState.components)

						condition = nextPredictedState.GetAllFacts()
						#add all of them after the relationship facts
						for fact in trueNextState.factsByComponentID[cid2]:
							if not isinstance(fact, PositionXFact) and not isinstance(fact, PositionYFact):
								minRelationshipFacts.append(fact)


						listAllFacts = []
						for fact in nextPredictedState.factsByComponentID[cid]:
							listAllFacts.append(fact)

						clonedEngine = engine.clone()
						clonedEngine.addRule(Rule(condition, EmptyFact(listAllFacts), EmptyFact(minRelationshipFacts)))

						if not clonedEngine in closedEngineList and not clonedEngine in neighbors:
							neighbors.append(clonedEngine)			


	print ("Neighbors: "+str(len(neighbors)))
	print ("Deleting engine Rules")
	neighbors = GenerateNeighborEngineDeletingRules(engine, closedEngineList, nextPredictedState,trueNextState,trueCurrState, openEngineHeapQ, neighbors)
	print ("Neighbors: "+str(len(neighbors)))
	return neighbors

def LearnEngine():
	#Load training data
	stateSequence = []
	gameName = "test"
	minFrame = 0
	maxFrame = 10#TODO; MUST GET THESE FROM EDITOR SIGNAL
	for frame in range(minFrame,maxFrame+1):
		source = open("./testing game 1/"+gameName+" "+str(frame)+".csv", "r")
		reader = csv.reader(source)
		readRow = False
		components = []
		for row in reader:

			if readRow:#Skip first row that defines width and height
				name = row[0]
				x = float(row[1])
				y = float(row[2])
				w = float(row[3])
				h = float(row[4])
				components.append([name,x,y,w,h])

			readRow = True
		stateSequence.append(State(components, (0,0,0), (0,0,0,0)))	
		source.close()

	#Set up delta facts
	'''
	for i in range(0,len(stateSequence)-1):
		stateSequence[i].SetupDeltaFacts(stateSequence[i+1])
	'''
	
	#Frame Scan
	allowedErrorRate = 0.0#domain dependent, based on component-finding and scaling noise
	startState = 0#Change this to use only a subset of the sequence
	currStateIndex = startState
	currState = stateSequence[currStateIndex]
	currEngine = Engine([])#pickle.load(open("learnedEngineFrame70Sequence1.p", "rb"))
	closedEngineList = []

	for stateIndex in range(0, len(stateSequence)-1):
		print ("Setting up state: "+str(stateIndex))
		state = stateSequence[stateIndex]
		nextState = stateSequence[stateIndex+1]
		state.SetupDeltaFacts(nextState)
	
	#Initial Sloppy Engine learning
	
	#Set initial velocity rules from the first frame
	'''
	firstStateFacts = stateSequence[0].GetAllFacts()
	firstStateFactsMinusVelocityFacts = []
	newVelocityRulePostEffects = []
	for fact in firstStateFacts:	
		if isinstance(fact, VelocityYFact) or isinstance(fact, VelocityXFact):
			if not fact.velocityValue==0:
				newVelocityRulePostEffects.append(fact)
		else:
			firstStateFactsMinusVelocityFacts.append(fact)

	for postEffect in newVelocityRulePostEffects:
		approximatedPreEffect = postEffect.clone()
		approximatedPreEffect.velocityValue = 0

		condition = list(firstStateFactsMinusVelocityFacts)
		condition.append(approximatedPreEffect)
		currEngine.addRule(Rule(condition, approximatedPreEffect, postEffect))
	'''
	remainingDifferencesInMappedComponents = True

	#Add velocity at final state (has to match # of velocity facts)
	for c in range(0, len(stateSequence[-1].components)):
		#TODO; just assume consistent velocity if there's a mapping
		stateSequence[-1].AddFact(VelocityXFact(c, 0))
		stateSequence[-1].AddFact(VelocityYFact(c, 0))
	'''
	while remainingDifferencesInMappedComponents:
		differenceNotFound = 0
		for stateIndex in range(0, len(stateSequence)-1):
			state = stateSequence[stateIndex]
			predState = currEngine.predict(state)#predict
			nextState = stateSequence[stateIndex+1]

			predState.SetupMappings(nextState)
			dist = PredictedStateDistance(predState, stateSequence[stateIndex+1])
			#Current to next mapping 
			components1To2Mappings = predState.components1To2Mappings
			#Next to current mapping
			components2To1Mappings = predState.components2To1Mappings

			#Find very first difference
			differenceFound = False
			preEffect = None
			postEffect = None
			for i in range(0, len(predState.components)):
				if i in predState.components1To2Mappings.keys() and predState.components1To2Mappings[i] in predState.components2To1Mappings.keys() and i==predState.components2To1Mappings[predState.components1To2Mappings[i]]:#if it matches
					for fact in predState.factsByComponentID[i]:
						#print ("	Fact: "+str(fact))
						if not isinstance(fact, RelationshipFactX) and not isinstance(fact, RelationshipFactY) and not isinstance(fact,PositionXFact) and not isinstance(fact, PositionYFact):
							matched = False
							closestFact = None
							for fact2 in nextState.factsByComponentID[predState.components1To2Mappings[i]]:
								if fact.CheckMatchBesidesID(fact2):
									matched = True
									break
								elif isinstance(fact, fact2.__class__):
									closestFact = fact2
							
							if not matched and not closestFact == None:
								preEffect = fact
								postEffect = closestFact
								differenceFound = True
								break
				else: #If i is not in the next state
					#disappear fact!
					listAllFacts = []
					for fact in predState.factsByComponentID[i]:
						listAllFacts.append(fact)
					preEffect = EmptyFact(listAllFacts)
					postEffect = EmptyFact([])
					differenceFound = True
					break

				if differenceFound:
					break

			if not differenceFound:
				for j in range(0, len(nextState.components)):#Appear
					if not j in predState.components2To1Mappings.keys():#unmatched, might have appeared
						minRelationshipFacts = nextState.GetMinRelationshipFactsToComponents(j, state.components)
						
						#add all of them after the relationship facts
						for fact in nextState.factsByComponentID[j]:
							if not isinstance(fact, PositionXFact) and not isinstance(fact, PositionYFact):
								minRelationshipFacts.append(fact)

						preEffect = EmptyFact([])
						postEffect = EmptyFact(minRelationshipFacts)


			if not differenceFound:
				differenceNotFound+=1
			if not preEffect==None:
				currEngine.addRule(Rule(predState.GetAllFacts(), preEffect, postEffect))
		if differenceNotFound==len(stateSequence)-1:
			remainingDifferencesInMappedComponents = False
	'''

	

	
	#Refinement
	currEngine = pickle.load(open("./engine-sloppyonly.p", "rb"))
	while currStateIndex<len(stateSequence)-1:
		currState.SetAction(stateSequence[currStateIndex].action)#Set action
		predictedState = currEngine.predict(currState)#Update facts from rules
		
		
		#Get error between predicted next frame and true next frame
		error = PredictedStateDistance(predictedState, stateSequence[currStateIndex+1], True)
		print ("Predicted Error: "+str(error)+" at frame "+str(currStateIndex)+" with action "+str(currState.action))

		if error<=allowedErrorRate:
			currStateIndex+=1
			currState = predictedState#predicted state was close enough to true next state
			
		else:
			#Engine Learning
			print ("Engine Learning")
			openEngineHeapQ = []
			heappush(openEngineHeapQ, (0, currEngine))
			unfinished = True
			
			currErrorToBeat = error
			currBest = currEngine
			iteration = 0


			while len(openEngineHeapQ)>0 and unfinished:
				engineTuple = heappop(openEngineHeapQ)
				print ("")
				print ("-POPPED NEW ENGINE-")

				engine = engineTuple[1]
				#for rule in engine.rules:           
				#	print ("	RULE: "+str(rule.preEffect)+" -> "+str(rule.postEffect))

				closedEngineList.append(engine)
				print ("")
				print ("prediction")
				predictedState = engine.predict(currState)

				#DrawFrame(currState.GetImageFromFacts(), 'currEngineCurrState'+str(iteration)+'.png')
				#DrawFrame(predictedState.GetImageFromFacts(), 'currEnginepredictedState'+str(iteration)+'.png')
				#DrawFrame(stateSequence[currStateIndex+1].observation, 'currEngineTrueNext'+str(iteration)+'.png')

				neighbors = GenerateNeighborEngines(engine,closedEngineList,predictedState, stateSequence[currStateIndex+1], currState, openEngineHeapQ)
				#print ("Found neighbors: "+str(len(neighbors)))
				numNeighbors = 0#test only
				for neighborEngine in neighbors:
					print ("  Checking Neighbor Engine "+str(numNeighbors)+" "+str(len(neighborEngine.rules)))
					#for rule in neighborEngine.rules:
					#	print ("	Rule: "+str(rule.preEffect)+"->"+str(rule.postEffect))
					
					neighborPredictedState = neighborEngine.predict(currState)

					#neighborPredictedFrame = neighborPredictedState.GetImageFromFacts()
					neighborError = PredictedStateDistance(neighborPredictedState, stateSequence[currStateIndex+1])
					print ("	Neighbor Error: "+str(neighborError))
					numNeighbors+=1 #test only

					if neighborError<currErrorToBeat:
						currErrorToBeat = neighborError
						currBest = neighborEngine
						print("		Best neighbor: "+str(currErrorToBeat))
						#for rule in neighborEngine.rules:
						#	print ("	Rule: "+str(rule.preEffect)+"->"+str(rule.postEffect))
						

					if neighborError<=allowedErrorRate:
						currEngine = neighborEngine
						unfinished = False
						break
					else:
						heappush(openEngineHeapQ, (neighborError, neighborEngine))
				iteration+=1
				if not unfinished:
					break
			if unfinished:#Somehow ran out, skip for now
				print ("FAILED")
				print ("")
				currState = stateSequence[currStateIndex+1]
				currStateIndex+=1
				currEngine = currBest
			else:#We learned enough, reset to ensure we didn't break anything
				print ("SUCCESS")
				print ("")
				currStateIndex = startState
				currState = stateSequence[currStateIndex]
				pickle.dump(currEngine, open("partialLearnedEngine.p", "wb"))
	
	return currEngine


	
def main():
	#Learn engine
	learnedEngine = LearnEngine()
	#Save final engine
	pickle.dump(learnedEngine, open("finalLearnedEngineLarge.p", "wb"))

if __name__ == '__main__':
	main()