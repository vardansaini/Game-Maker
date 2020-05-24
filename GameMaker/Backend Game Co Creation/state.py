import numpy as np 
from facts import *

'''

Refine only 2:30 with 10 frames
Sloppy only 30 seconds with 10 frames

Helper functions
name, x, y, w h
'''
def GetNorthPoint(component):
	return [component[1]+component[3]/2.0,component[2]]

def GetSouthPoint(component):
	return [component[1]+component[3]/2.0,component[2]+component[4]]

def GetEastPoint(component):
	return [component[1]+component[3],component[2]+component[4]/2.0]

def GetWestPoint(component):
	return [component[1],component[2]+component[4]/2.0]

def GetCenterPoint(component):
	return [component[1]+component[3]/2.0,component[2]+component[4]/2.0]


def GetConnectionPointPosition(component, connectionPoint):
	if "North"==connectionPoint:
		return GetNorthPoint(component)
	elif "South"==connectionPoint:
		return GetSouthPoint(component)
	elif "East"==connectionPoint:
		return GetEastPoint(component)
	elif "West"==connectionPoint:
		return GetWestPoint(component)
	else:
		return GetCenterPoint(component)

def GetTopLeftCorner(component1, relationshipX, relationshipY ,cw2, ch2):
	#X Position
	connectPoint1X = GetConnectionPointPosition(component1, relationshipX.connectPoint)
	xPos = connectPoint1X[0]-relationshipX.distance

	if "North"==relationshipX.connectPointOther:
		xPos-=cw2/2.0
	elif "South"==relationshipX.connectPointOther:
		xPos-=cw2/2.0
	elif "East"==relationshipX.connectPointOther:
		xPos-=cw2
	elif "West"==relationshipX.connectPointOther:
		xPos-=0
	else:
		xPos-=cw2/2.0

	#Y Position
	connectPoint1Y = GetConnectionPointPosition(component1, relationshipY.connectPoint)
	yPos = connectPoint1Y[1]-relationshipY.distance

	if "North"==relationshipY.connectPointOther:
		yPos-=0
	elif "South"==relationshipY.connectPointOther:
		yPos-=ch2
	elif "East"==relationshipY.connectPointOther:
		yPos-=ch2/2.0
	elif "West"==relationshipY.connectPointOther:
		yPos-=ch2/2.0
	else:
		yPos-=ch2/2.0

	return xPos, yPos


def GetEdgeX(component1, component2, cid1, cid2):
	edgeName = ["North", "South", "East", "West", "Center"]
	edge1_options = [GetNorthPoint(component1), GetSouthPoint(component1), GetEastPoint(component1), GetWestPoint(component1), GetCenterPoint(component1)]
	edge2_options = [GetNorthPoint(component2), GetSouthPoint(component2), GetEastPoint(component2), GetWestPoint(component2), GetCenterPoint(component2)]

	minDist = float("inf")
	actualConnection = 0
	point1 = ""
	point2 = ""
	for i in range(0, len(edge1_options)):
		for j in range(0, len(edge2_options)):
			#This can be abs or it doesn't let you get the ordering
			dist = (edge1_options[i][0]-edge2_options[j][0])
			if abs(dist)<minDist:
				point1 = edgeName[i]
				point2 = edgeName[j]
				minDist = abs(dist)
				actualConnection = dist
	topLeft = GetTopLeftCorner(component1, RelationshipFactX(cid1, cid2, point1, point2, actualConnection), RelationshipFactX(cid1, cid2, point1, point2, actualConnection) ,component2[2].shape[0], component2[2].shape[1])
	return RelationshipFactX(cid1, cid2, point1, point2, actualConnection)

def GetEdgeY(component1, component2, cid1, cid2):
	edgeName = ["North", "South", "East", "West", "Center"]
	edge1_options = [GetNorthPoint(component1), GetSouthPoint(component1), GetEastPoint(component1), GetWestPoint(component1), GetCenterPoint(component1)]
	edge2_options = [GetNorthPoint(component2), GetSouthPoint(component2), GetEastPoint(component2), GetWestPoint(component2), GetCenterPoint(component2)]

	minDist = float("inf")
	actualConnection = 0
	point1 = ""
	point2 = ""
	for i in range(0, len(edge1_options)):
		for j in range(0, len(edge2_options)):
			dist = (edge1_options[i][1]-edge2_options[j][1])
			if abs(dist)<minDist:
				point1 = edgeName[i]
				point2 = edgeName[j]
				minDist = abs(dist)
				actualConnection = dist
	return RelationshipFactY(cid1, cid2, point1, point2, actualConnection)

#Distance metric for components
def ComponentToComponentHellmansMetric(component1,component2):
	dist = 0

	if not component1[0]==component2[0]:
		dist+=100

	dist+=abs(component1[1]-component2[1])#x dist
	dist+=abs(component1[2]-component2[2])#y dist

	dist+=abs(component1[3]-component2[3])#w diff
	dist+=abs(component1[4]-component2[4])#h diff

	return dist

#Get mappings between components
def GetMapping(components1, components2):
	#Current to next state matching
	componentsMatchings = {}
	for c2 in range(0, len(components2)):
		minDist = float("inf")
		componentsMatchings[c2] = []
		for c1 in range(0, len(components1)):
			dist = ComponentToComponentHellmansMetric(components1[c1], components2[c2])
			componentsMatchings[c2].append([dist, c1])
			
	#sort
	for c2 in range(0, len(components2)):
		componentsMatchings[c2] = sorted(componentsMatchings[c2])

	#Find the best unique mapping for each c2
	finalMappings = {}
	numToMatch = min(len(components1), len(components2))
	takenc2s = []
	takenc1s = []
	while numToMatch>0:
		bestUnmatched = -1
		minDist = float("inf")
		bestc2 = -1
		for c2 in range(0, len(components2)):
			if not c2 in takenc2s:
				for valPair in componentsMatchings[c2]:
					if not valPair[1] in finalMappings.keys():
						if valPair[0]<minDist:
							minDist = valPair[0]
							bestUnmatched = valPair[1]
							bestc2 = c2

		finalMappings[bestUnmatched] = bestc2
		takenc2s.append(bestc2)
		takenc1s.append(bestUnmatched)
		numToMatch-=1
	return finalMappings

'''

Basic state representation
Input: List of components and their locations on screen, the background color, the reward, and the current action

'''
class State:
	def __init__(self, _components, _bgColor, _action):
		self.components = _components#x,y,component
		self.bgColor = _bgColor
		self.action = _action

		#Initially empty, but will become the fact representation
		self.factsByComponentID = {}

		self.SetUpBaseFacts()

		#Create variable facts
		#TODO; readd this
		#self.AddFact(VariableFact("bgColor", self.bgColor))

		#space up down left right
		self.AddFact(VariableFact("space", self.action[0]))
		self.AddFact(VariableFact("up", self.action[1]))
		self.AddFact(VariableFact("down", self.action[2]))
		self.AddFact(VariableFact("left", self.action[3]))
		self.AddFact(VariableFact("right", self.action[4]))

		#Current to next mapping
		self.components1To2Mappings = {}
		#Next to current mapping
		self.components2To1Mappings = {}


	def GetLargestComponentID(self):#When components have not yet been updated but we need to update the largest componentID
		minID = len(self.components)-1
		for compID in self.factsByComponentID.keys():
			if compID>minID:
				minID=compID
		return minID

	def CreateRelationshipFacts(self):
		#Create relationship x and y facts
		for c in range(0, len(self.components)):
			for c2 in range(0, len(self.components)):
				if not c==c2:
					#Determine relationship x 
					f1 = GetEdgeX(self.components[c],self.components[c2],c,c2)
					self.relationshipFacts.add(f1)
					#Determine relationship y
					f2 = GetEdgeY(self.components[c],self.components[c2],c,c2)
					self.relationshipFacts.add(f2)

	def GetMinRelationshipFactsToComponents(self, componentID, components, excluding = -1):
		minX = None
		minY = None
		minDist = float("inf")
		
		for c in range(0, len(components)):
			if not c==componentID and not c==excluding:
				f1 = GetEdgeX(components[c], self.components[componentID], c, componentID)
				f2 = GetEdgeY(components[c], self.components[componentID], c, componentID)
				if f1.distance+f2.distance<minDist:
					minDist = f1.distance+f2.distance
					minX = f1
					minY = f2

		return [minX, minY]


	def GetRelationshipFacts(self):
		if len(self.relationshipFacts)==0:
			self.CreateRelationshipFacts()
		return self.relationshipFacts
		

	def SetUpBaseFacts(self):
		#Create animation, spatial facts
		for c in range(0, len(self.components)):
			f = AnimationFact(c, self.components[c][0], self.components[c][3], self.components[c][4])
			self.AddFact(f)
			f = PositionXFact(c, self.components[c][1])
			self.AddFact(f)
			f = PositionYFact(c, self.components[c][2])
			self.AddFact(f)

	def SetAction(self, action):
		for fact in self.factsByComponentID[-1]:
			if isinstance(fact,VariableFact):
				if fact.variableName =="action":
					fact.variableValue = action
		self.action = action

	def AddFact(self, otherFact, printIt = False):

		if not otherFact.componentID in self.factsByComponentID.keys():
			self.factsByComponentID[otherFact.componentID] = []
		if not otherFact in self.factsByComponentID[otherFact.componentID]: 
			if (isinstance(otherFact.__class__, VelocityXFact)):
				velocityXFactInThere = False
				for existingFact in self.factsByComponentID[otherFact.componentID]:
					if isinstance(existingFact, VelocityXFact):
						velocityXFactInThere = True
						break
			if printIt:
				print ("Adding Other Fact: "+str(otherFact))
			self.factsByComponentID[otherFact.componentID].append(otherFact)

	# To be called after prediction to update components and then facts to fit fact changes
	def Update(self):
		#Collect all the individual components 
		componentDict = {}
		for fList in self.factsByComponentID.values():
			for f in fList:
				if isinstance(f,AnimationFact):#Found animation fact
					if len(f.name)>0:
						if not f.componentID in componentDict.keys():
							componentDict[f.componentID] = [f.name, -1, -1, f.width, f.height]
						else:
							componentDict[f.componentID][0] = f.name
							componentDict[f.componentID][3] = f.width
							componentDict[f.componentID][4] = f.height
				elif isinstance(f, PositionXFact):
					if not f.componentID in componentDict.keys():
						componentDict[f.componentID] = ["", f.posX, -1, -1, -1]
					else:
						componentDict[f.componentID][1] = f.posX
				elif isinstance(f, PositionYFact):
					if not f.componentID in componentDict.keys():
						componentDict[f.componentID] = ["", -1, f.posY, -1, -1]
					else:
						componentDict[f.componentID][2] = f.posY

		#Update components
		idUpdates = {}
		self.components = []
		for componentKey in componentDict.keys():
			idUpdates[componentKey] = len(self.components)
			self.components.append(componentDict[componentKey])
			#print ("UPDATE COMPONENT: "+str(componentDict[componentKey]))
		#Grab non-spatial facts, as they are assumed to be constant if not otherwise changing
		newFactsByComponentID = {}
		for cid in self.factsByComponentID.keys():
			for fact in self.factsByComponentID[cid]:
				if isinstance(fact,VariableFact) or isinstance(fact,VelocityXFact) or isinstance(fact,VelocityYFact):
					newFact = fact.clone()
					if newFact.componentID in idUpdates.keys():
						newFact.componentID = idUpdates[newFact.componentID]
					if not newFact.componentID in newFactsByComponentID.keys():
						newFactsByComponentID[newFact.componentID] = []
					if isinstance(fact,VelocityXFact):
						velocityXFactInThere = False
						for presentFact in newFactsByComponentID[newFact.componentID]:
							if isinstance(presentFact, VelocityXFact):
								velocityXFactInThere = True
								break
						if velocityXFactInThere:
							print ("FUCKIN DOUBLE VELOCITY: "+str(newFact))

					newFactsByComponentID[newFact.componentID].append(newFact)

		#Reset base facts
		self.factsByComponentID = newFactsByComponentID
		self.SetUpBaseFacts()

		#TODO; Reset relationship facts

	#Collect spatial facts and animation facts (this is inefficient)
	def GetComponentsFromFactsByID(self):
		componentsByID = {}
		for cid in self.factsByComponentID.keys():
			componentsByID[cid] = ["", -1,-1,-1, -1]
			for f in self.factsByComponentID[cid]:
				if isinstance(f, AnimationFact):
					componentsByID[cid][0] = f.name
					componentsByID[cid][3] = f.width
					componentsByID[cid][4] = f.height
				elif isinstance(f, PositionXFact):
					componentsByID[cid][1] = f.posX
				elif isinstance(f, PositionYFact):
					componentsByID[cid][2] = f.posY
		return componentsByID

	def GetSingleComponentFromFactsByID (self, cid):
		component = ["", -1,-1,-1, -1]
		for f in self.factsByComponentID[cid]:
			if isinstance(f, AnimationFact):
				component[0] = f.name
				component[3] = f.width
				component[4] = f.height
			elif isinstance(f, PositionXFact):
				component[1] = f.posX
			elif isinstance(f, PositionYFact):
				component[2] = f.posY
		return component

	def clone(self):
		newState = State(list(self.components), self.bgColor, self.action)
		for cid in self.factsByComponentID.keys():
			for fact in self.factsByComponentID[cid]:
				newState.AddFact(fact.clone())

		#Current to next mapping
		newState.components1To2Mappings = dict(self.components1To2Mappings)
		#Next to current mapping
		newState.components2To1Mappings = dict(self.components2To1Mappings)

		return newState

	def GetAllFacts(self):
		facts = []
		for cid in self.factsByComponentID.keys():
			facts +=self.factsByComponentID[cid]
		return facts

	def SetupMappings(self, nextState):
		#Current to next mapping
		self.components1To2Mappings = GetMapping(self.components, nextState.components)
		#Next to current mapping
		self.components2To1Mappings = GetMapping(nextState.components, self.components)

	#This function adds the velocity and animation facts
	def SetupDeltaFacts(self, nextState):
		#Set up curr to next
		self.SetupMappings(nextState)
		
		#Create velocity facts based on mapping
		for i in range(0, len(self.components)):

			#Remove Velocity Facts if they're already in there
			toRemove = []
			for fact in self.factsByComponentID[i]:
				if isinstance(fact, VelocityXFact) or isinstance(fact, VelocityYFact):
					toRemove.append(fact)
			for fact in toRemove:
				self.factsByComponentID[i].remove(fact)

			if i in self.components1To2Mappings.keys() and self.components1To2Mappings[i] in self.components2To1Mappings.keys() and i==self.components2To1Mappings[self.components1To2Mappings[i]]:#if it matches
				#print ("Adding Velocity Facts: "+str([nextState.components[self.components1To2Mappings[i]][1]-self.components[i][1], nextState.components[self.components1To2Mappings[i]][2]-self.components[i][2]]) +" due to component: "+str(self.components[i])+" mapped to "+str(nextState.components[self.components1To2Mappings[i]]))
				self.AddFact(VelocityXFact(i, nextState.components[self.components1To2Mappings[i]][1]-self.components[i][1]))
				self.AddFact(VelocityYFact(i, nextState.components[self.components1To2Mappings[i]][2]-self.components[i][2]))
			else:
				self.AddFact(VelocityXFact(i, 0))
				self.AddFact(VelocityYFact(i, 0))
	'''
	def SetupEmptyFacts(self, nextState):
		#Create appearing/disappearing based on mapping
		if len(self.components)>len(nextState.components):
			unmatched = []
			for c in range(0, len(self.components)):
				if not c in self.components2To1Mappings.values():
					unmatched.append(self.components[c])
			numAdded =1
			for unmatchedComponent in unmatched:
				nextState.AddFact(AnimationFact(len(nextState.components)+numAdded, unmatchedComponent[0], -1, -1))
				nextState.AddFact(PositionXFact(len(nextState.components)+numAdded, unmatchedComponent[1]))
				nextState.AddFact(PositionYFact(len(nextState.components)+numAdded, unmatchedComponent[2]))
				nextState.AddFact(VelocityXFact(len(nextState.components)+numAdded, 0))
				nextState.AddFact(VelocityYFact(len(nextState.components)+numAdded, 0))
				numAdded+=1
		else:
			unmatched = []
			for c2 in range(0, len(nextState.components)):
				if not c2 in self.components1To2Mappings.values():
					unmatched.append(nextState.components[c2])
			numAdded =1
			maxComponentId = self.GetLargestComponentID()
			for unmatchedComponent in unmatched:
				self.AddFact(AnimationFact(maxComponentId+numAdded, unmatchedComponent[0], -1, -1))
				self.AddFact(PositionXFact(maxComponentId+numAdded, unmatchedComponent[1]))
				self.AddFact(PositionYFact(maxComponentId+numAdded, unmatchedComponent[2]))
				self.AddFact(VelocityXFact(maxComponentId+numAdded, 0))
				self.AddFact(VelocityYFact(maxComponentId+numAdded, 0))
				numAdded+=1
	'''


