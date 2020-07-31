'''
Engine stuff

'''
import numpy as np 
from facts import *
from state import *

class Rule:
	'''
	Conditions are facts that must be in a state for the rule to fire
	preEffect is the fact that you replace postEffect with
	preEffect must be inside the conditions list
	'''
	def __init__(self, _conditions, _preEffect, _postEffect):
		self.conditions = _conditions
		self.preEffect = _preEffect
		self.postEffect = _postEffect
		self.framesWhereRuleCouldHaveFired = []

		self.conditionsByID = {}
		for cond in self.conditions:
			if not cond.componentID in self.conditionsByID.keys():
				self.conditionsByID[cond.componentID] = []
			self.conditionsByID[cond.componentID].append(cond)

	def __eq__(self, other):
		if isinstance(other, self.__class__):
			return self.conditions==other.conditions and self.preEffect==other.preEffect and self.postEffect==other.postEffect
		else:
			return False

	def __ne__(self, other):
		if isinstance(other, self.__class__):
			return not self.__eq__(other)
		else:
			return True

	def __hash__(self):
		return hash(tuple(["Rule", self.conditions, self.preEffect, self.postEffect]))

	def clone(self):
		cloneConditions = []
		for c in self.conditions:
			cloneConditions.append(c.clone())
		clonePreEffect = self.preEffect.clone()
		clonePostEffect = self.postEffect.clone()
		return Rule(cloneConditions, clonePreEffect, clonePostEffect)

	def ConditionSatisfiedCheck(self, state):
		#Check if all of the shared facts for each componentID in this rule are matched in this state, if any don't return false
		effectIds = []
		for cKey in self.conditionsByID.keys():
			componentIDs = []
			for ruleFact in self.conditionsByID[cKey]:
				#print ("ConditionSatisfiedCheck checking "+str(ruleFact))
				#Collect the potential componentID matches
				if len(componentIDs)==0:
					for stateFact in state.GetAllFacts():#Find all matching facts in state
						if not stateFact.componentID in componentIDs and ruleFact.CheckMatchBesidesID(stateFact):
							#print ("Found match: :"+str(stateFact))
							componentIDs.append(stateFact.componentID)
					if len(componentIDs)==0:
						#print ("Failed on 1: "+str(ruleFact))
						return False, []
				else:
					newComponentIDs = []
					for compId in componentIDs:
						for stateFact in state.factsByComponentID[compId]:
							#print ("	StateFact: "+str(stateFact))
							if not stateFact.componentID in newComponentIDs and ruleFact.CheckMatchBesidesID(stateFact):
								#print ("Found match: :"+str(stateFact))
								newComponentIDs.append(stateFact.componentID)

					if len(newComponentIDs)==0:
						#print ("Failed on 2: "+str(ruleFact)+" conditions: "+str(componentIDs))
						return False, []
					else:
						componentIDs = newComponentIDs
			if isinstance(self.preEffect, EmptyFact):
				if len(self.preEffect.replacementFacts)>0:#Must ensure all of these in here
					if cKey == self.preEffect.componentID:
						for cid in componentIDs:
							#Ensure this one totally matches
							if not cid in effectIds:
								totalMatch = True
								for vanishFact in self.preEffect.replacementFacts:
									trueID = vanishFact.componentID
									vanishFact.componentID = cid
									if not vanishFact in state.factsByComponentID[cid]:
										vanishFact.componentID = trueID
										totalMatch = False
										break
									else:
										vanishFact.componentID = trueID
								if totalMatch:
									effectIds.append(cid)
				if len(self.postEffect.replacementFacts)>0 and len(self.preEffect.replacementFacts)==0:
					if cKey == self.postEffect.componentID:
						for cid in componentIDs:
							if not cid in effectIds:
								effectIds.append(cid)
			else:
				if cKey == self.preEffect.componentID:
					for cid in componentIDs:
						if not cid in effectIds:
							effectIds.append(cid)
		return True, effectIds

class Engine:
	def __init__(self, _rules):
		self.rules=_rules#ordered list of rules to run

	def __eq__(self, other):
		if isinstance(other, self.__class__):
			return self.rules==other.rules
		else:
			return False

	def __ne__(self, other):
		if isinstance(other, self.__class__):
			return not self.__eq__(other)
		else:
			return True

	def __hash__(self):
		return hash(tuple(rules))

	def GetComplexity(self):
		complexity = 0
		for rule in self.rules:
			complexity+=len(rule.conditions)
		return complexity

	def __lt__(self, other):
		return self.GetComplexity()<other.GetComplexity()

	def clone(self):
		cloneRules = []
		for rule in self.rules:
			cloneRules.append(rule.clone())
		
		return Engine(cloneRules)

	def addRule(self,rule):
		self.rules.append(rule)

	def ruleActivation(self, predictionState):
		#Attempt to run each rule
		for rule in self.rules:
			#print ("RULE: "+str(rule.preEffect)+" -> "+str(rule.postEffect))

			matched, conditionIds = rule.ConditionSatisfiedCheck(predictionState)
			if matched:
				#for cond in rule.conditions:
				#	print (" 	Satisfied Conditions: :"+str(cond))
				#print ("	RULE ACTIVATED")
				if isinstance(rule.preEffect, EmptyFact) and isinstance(rule.postEffect, EmptyFact):
					if len(rule.preEffect.replacementFacts)>0 and len(rule.postEffect.replacementFacts)==0:
						#Disappear
						for cid in conditionIds:
							#Remove each fact in here
							for fact in rule.preEffect.replacementFacts:
								trueID = fact.componentID
								fact.componentID = cid
								predictionState.factsByComponentID[cid].remove(fact)
								fact.componentID = trueID
							#if empty, delete
							if len(predictionState.factsByComponentID[cid])==0:
								del predictionState.factsByComponentID[cid]
					elif len(rule.postEffect.replacementFacts)>0 and len(rule.preEffect.replacementFacts)==0:
						for cid in conditionIds:
							#Appear
							newComponentID = predictionState.GetLargestComponentID()+1
							#turn relationship facts into positions facts

							animationFact = None
							for i in range(2, len(rule.postEffect.replacementFacts)):
								if isinstance(rule.postEffect.replacementFacts[i], AnimationFact):
									animationFact = rule.postEffect.replacementFacts[i]
									break

							#add the rest
							posx, posy = GetTopLeftCorner(predictionState.GetSingleComponentFromFactsByID(cid), rule.postEffect.replacementFacts[0], rule.postEffect.replacementFacts[1], animationFact.width, animationFact.height)
							posXFact = PositionXFact(newComponentID, posx)
							posYFact = PositionYFact(newComponentID, posy)
							if not predictionState.AnythingAtPosition(posXFact, posYFact):
								print("Nothing was at this position: "+str(posx)+", "+str(posy))
								predictionState.AddFact(posXFact)
								predictionState.AddFact(posYFact)
								for i in range(2, len(rule.postEffect.replacementFacts)):
									cloneFact = rule.postEffect.replacementFacts[i].clone()
									cloneFact.componentID = newComponentID
									predictionState.AddFact(cloneFact)
					else:
						for cid in conditionIds:
							#Appear
							newComponentID = cid
							#turn relationship facts into positions facts

							animationFact = None
							for i in range(2, len(rule.postEffect.replacementFacts)):
								if isinstance(rule.postEffect.replacementFacts[i], AnimationFact):
									animationFact = rule.postEffect.replacementFacts[i]
									break

							#add the rest
							posx, posy = GetTopLeftCorner(predictionState.GetSingleComponentFromFactsByID(cid), rule.postEffect.replacementFacts[0], rule.postEffect.replacementFacts[1], animationFact.width, animationFact.height)

							#Remove each fact in here
							for fact in rule.preEffect.replacementFacts:
								trueID = fact.componentID
								fact.componentID = cid
								predictionState.factsByComponentID[cid].remove(fact)
								fact.componentID = trueID

							predictionState.AddFact(PositionXFact(newComponentID, posx))
							predictionState.AddFact(PositionYFact(newComponentID, posy))
							for i in range(2, len(rule.postEffect.replacementFacts)):
								cloneFact = rule.postEffect.replacementFacts[i].clone()
								cloneFact.componentID = newComponentID
								predictionState.AddFact(cloneFact)

				else:
					for cid in conditionIds:
						factsToReplace = [] 
						for fact in predictionState.factsByComponentID[cid]:
							if not fact in factsToReplace and rule.preEffect.CheckMatchBesidesID(fact):
								factsToReplace.append(fact)

						for oldFact in factsToReplace:
							currID = oldFact.componentID
							predictionState.factsByComponentID[cid].remove(oldFact)
							newFact = rule.postEffect.clone()
							newFact.componentID = currID
							predictionState.AddFact(newFact)
		return predictionState

	#Alters state with the set of current rules then updates that state
	def predict(self, state):
		predictionState=state.clone()
		predictionState = self.ruleActivation(predictionState)
		predictionState = self.velocityUpdate(predictionState)
		predictionState.Update()
		return predictionState

	def predictNoVelocityUpdate(self, state):
		predictionState=state.clone()
		predictionState = self.ruleActivation(predictionState)
		predictionState.Update()
		return predictionState
	
	#TODO; make more efficient
	def velocityUpdate(self, predictionState):
		#Activate any remaining velocity
		velocityFacts = []
		for fact in predictionState.GetAllFacts():
			if isinstance(fact, VelocityXFact) or isinstance(fact,VelocityYFact):
				if abs(fact.velocityValue)>0 or abs(fact.velocityValue)<0:
					velocityFacts.append(fact)
		for fact in predictionState.GetAllFacts():
			for velocityFact in velocityFacts:

				if isinstance(fact,PositionXFact):
					if velocityFact.componentID==fact.componentID:
						if isinstance(velocityFact, VelocityXFact):
							fact.posX+=velocityFact.velocityValue#Update position
							#print ("Position Updating: "+str(fact)+" with "+str(velocityFact))
				if isinstance(fact, PositionYFact):
					if velocityFact.componentID==fact.componentID:
						if isinstance(velocityFact, VelocityYFact):
							fact.posY+=velocityFact.velocityValue#Update position
							#print ("Position Updating: "+str(fact)+" with "+str(velocityFact))
		return predictionState






