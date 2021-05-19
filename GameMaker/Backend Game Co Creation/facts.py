import numpy as np
'''

Fact types

'''

#The basic fact concept
class Fact(object):
	def __init__(self, _componentID):
		self.componentID = _componentID

	def CheckMatchBesidesID(self, other):
		trueID = self.componentID
		self.componentID = other.componentID
		match = self==other
		self.componentID = trueID
		return match

	def GetValue(self):
		return -1

class VelocityXFact(Fact):
	def __init__(self, _componentID, _velocityVal):
		super(VelocityXFact,self).__init__(_componentID)
		self.velocityValue = _velocityVal

	def __eq__(self, other):
		if isinstance(other, self.__class__):
			return self.componentID==other.componentID and self.velocityValue==other.velocityValue
		else:
			return False

	def __ne__(self, other):
		if isinstance(other, self.__class__):
			return not self.__eq__(other)
		else:
			return True

	def __hash__(self):
		return hash(tuple(["VelocityXFact", self.componentID, self.velocityValue]))

	def clone(self):
		fact = VelocityXFact(self.componentID, self.velocityValue)
		return fact

	def __str__(self):
		return "VelocityXFact: "+str([self.componentID, self.velocityValue])

	def GetValue(self):
		return self.velocityValue

class VelocityYFact(Fact):
	def __init__(self, _componentID, _velocityVal):
		super(VelocityYFact,self).__init__(_componentID)
		self.velocityValue = _velocityVal

	def __eq__(self, other):
		if isinstance(other, self.__class__):
			return self.componentID==other.componentID and self.velocityValue==other.velocityValue
		else:
			return False

	def __ne__(self, other):
		if isinstance(other, self.__class__):
			return not self.__eq__(other)
		else:
			return True

	def __hash__(self):
		return hash(tuple(["VelocityYFact", self.componentID, self.velocityValue]))

	def clone(self):
		fact = VelocityYFact(self.componentID, self.velocityValue)
		return fact

	def __str__(self):
		return "VelocityYFact: "+str([self.componentID, self.velocityValue])

	def GetValue(self):
		return self.velocityValue


#Animation State, position, and not exists
class AnimationFact(Fact):
	def __init__(self, _componentID, _name, _width, _height):
		super(AnimationFact,self).__init__(_componentID)
		self.name = _name
		self.width = _width
		self.height = _height

	def __eq__(self, other):
		if isinstance(other, self.__class__):
			return self.name==other.name and self.componentID==other.componentID and self.width==other.width and self.height==other.height
		else:
			return False

	def __ne__(self, other):
		if isinstance(other, self.__class__):
			return not self.__eq__(other)
		else:
			return True

	def __hash__(self):
		return hash(tuple(["AnimationFact", self.name, self.width, self.height, self.componentID]))

	def clone(self):
		fact = AnimationFact(self.componentID, self.name, self.width, self.height)
		return fact

	def __str__(self):
		return "AnimationFact: "+str([self.componentID, self.name, self.width, self.height])

	def GetValue(self):
		return self.shape

#spatial location of facts
class PositionXFact(Fact):
	def __init__(self, _componentID, _posX):
		super(PositionXFact,self).__init__(_componentID)
		self.posX = _posX

	def __eq__(self, other):
		if isinstance(other, self.__class__):
			return self.componentID==other.componentID and self.posX==other.posX
		else:
			return False

	def __ne__(self, other):
		if isinstance(other, self.__class__):
			return not self.__eq__(other)
		else:
			return True

	def __hash__(self):
		return hash(tuple(["PositionXFact", self.componentID, self.posX]))

	def clone(self):
		fact = PositionXFact(self.componentID, self.posX)
		return fact

	def __str__(self):
		return "PositionXFact: "+str([self.componentID, self.posX])

	def GetValue(self):
		return self.posX

#spatial location of facts
class PositionYFact(Fact):
	def __init__(self, _componentID, _posY):
		super(PositionYFact,self).__init__(_componentID)
		self.posY = _posY

	def __eq__(self, other):
		if isinstance(other, self.__class__):
			return self.componentID==other.componentID and self.posY==other.posY
		else:
			return False

	def __ne__(self, other):
		if isinstance(other, self.__class__):
			return not self.__eq__(other)
		else:
			return True

	def __hash__(self):
		return hash(tuple(["PositionYFact", self.componentID, self.posY]))

	def clone(self):
		fact = PositionYFact(self.componentID, self.posY)
		return fact

	def __str__(self):
		return "PositionYFact: "+str([self.componentID, self.posY])

	def GetValue(self):
		return self.posY

#Variable facts (e.g. actions, score)
class VariableFact(Fact):
	def __init__(self, variableName, variableValue):
		super(VariableFact,self).__init__(-1)
		self.variableName = variableName
		self.variableValue = variableValue

	def __eq__(self, other):
		if isinstance(other, self.__class__):
			return str(self.variableValue)==str(other.variableValue) and self.variableName==other.variableName
		else:
			return False

	def __ne__(self, other):
		if isinstance(other, self.__class__):
			return not self.__eq__(other)
		else:
			return True

	def __hash__(self):
		return hash(tuple([self.variableName, str(self.variableValue)]))

	def clone(self):
		fact = VariableFact(self.variableName, self.variableValue)
		return fact

	def __str__(self):
		return "VariableFact: "+str([self.variableName, self.variableValue])

	def GetValue(self):
		return self.variableValue


#Hierarchical Facts

#relationships between sprites
class RelationshipFactX(Fact):
	def __init__(self, _componentID1, _componentID2, _connectPoint1, __connectPoint2, _distance):
		super(RelationshipFactX,self).__init__(_componentID1)
		self.componentIDOther = _componentID2
		self.connectPoint = _connectPoint1
		self.connectPointOther = __connectPoint2
		self.distance = _distance

	def CheckMatchBesidesID(self, other):
		if isinstance(other, self.__class__):
			trueID = self.componentID
			self.componentID = other.componentID

			secondTrueID = self.componentIDOther
			self.componentIDOther = other.componentIDOther

			match = self==other
			self.componentID = trueID

			self.componentIDOther = secondTrueID

			return match
		else:
			return False

	def __eq__(self, other):
		if isinstance(other, self.__class__):
			return self.componentID==other.componentID and self.componentIDOther==other.componentIDOther and self.connectPoint==other.connectPoint and self.connectPointOther==other.connectPointOther and self.distance==other.distance
		else:
			return False

	def __ne__(self, other):
		if isinstance(other, self.__class__):
			return not self.__eq__(other)
		else:
			return True

	def __hash__(self):
		return hash(tuple(["RelationshipFactX", self.componentID, self.componentIDOther, self.connectPoint, self.connectPointOther, self.distance]))

	def clone(self):
		fact = RelationshipFactX(self.componentID, self.componentIDOther, self.connectPoint, self.connectPointOther, self.distance)
		return fact

	def __str__(self):
		return "RelationshipFactX: "+str([self.componentID, self.componentIDOther, self.connectPoint, self.connectPointOther, self.distance])

	def GetValue(self):
		return self.distance

#relationships between sprites
class RelationshipFactY(Fact):
	def __init__(self, _componentID1, _componentID2, _connectPoint1, __connectPoint2, _distance):
		super(RelationshipFactY,self).__init__(_componentID1)
		self.componentIDOther = _componentID2
		self.connectPoint = _connectPoint1
		self.connectPointOther = __connectPoint2
		self.distance = _distance

	def CheckMatchBesidesID(self, other):
		if isinstance(other, self.__class__):
			trueID = self.componentID
			self.componentID = other.componentID

			secondTrueID = self.componentIDOther
			self.componentIDOther = other.componentIDOther

			match = self==other
			self.componentID = trueID

			self.componentIDOther = secondTrueID

			return match
		else:
			return False

	def __eq__(self, other):
		if isinstance(other, self.__class__):
			return self.componentID==other.componentID and self.componentIDOther==other.componentIDOther and self.connectPoint==other.connectPoint and self.connectPointOther==other.connectPointOther and self.distance==other.distance
		else:
			return False

	def __ne__(self, other):
		if isinstance(other, self.__class__):
			return not self.__eq__(other)
		else:
			return True

	def __hash__(self):
		return hash(tuple(["RelationshipFactY", self.componentID, self.componentIDOther, self.connectPoint, self.connectPointOther, self.distance]))

	def clone(self):
		fact = RelationshipFactY(self.componentID, self.componentIDOther, self.connectPoint, self.connectPointOther, self.distance)
		return fact

	def __str__(self):
		return "RelationshipFactY: "+str([self.componentID, self.componentIDOther, self.connectPoint, self.connectPointOther, self.distance])

	def GetValue(self):
		return self.distance

class InequalityFact(Fact):
	def __init__(self, _fact, _value, _relationship):
		super(InequalityFact,self).__init__(_fact.componentID)
		self.fact = _fact
		self.value = _value
		self.relationship = _relationship#string either >= or  <=

	def CheckMatchBesidesID(self, other):#other in this case is not InequalityFact
		if isinstance(other, self.fact.__class__):
			otherVal = other.GetValue()

			if self.relationship==">=" and otherVal>=self.value:
				return True
			elif self.relationship=="<=" and otherVal<=self.value:
				return True
			else:
				return False
		else:
			return False

	def __eq__(self, other):
		if isinstance(other, self.__class__):
			return self.fact==other.fact and self.value==other.value and self.relationship == other.relationship
		else:
			return False

	def __ne__(self, other):
		if isinstance(other, self.__class__):
			return not self.__eq__(other)
		else:
			return True

	def __hash__(self):
		return hash(tuple(["InequalityFact", hash(self.fact), self.value, self.relationship]))

	def clone(self):
		fact = InequalityFact(self.fact, self.value, self.relationship)
		return fact

	def __str__(self):
		return "InequalityFact: "+str([str(self.fact), self.value, self.relationship])

class EmptyFact(Fact):
	def __init__(self, _replacementFacts):
		if len(_replacementFacts)>0 and (not _replacementFacts[0]==None):
			super(EmptyFact,self).__init__(_replacementFacts[0].componentID)
		else:
			super(EmptyFact,self).__init__(-1)
		self.replacementFacts = _replacementFacts

	def CheckMatchBesidesID(self, other):#other in this case is not EmptyFact
		if isinstance(other, self.__class__):
			if len(self.replacementFacts)==len(other.replacementFacts):
				toMatch = len(self.replacementFacts)
				otherMatched = []
				for fact in self.replacementFacts:
					for fact2 in other.replacementFacts:
						if not fact2 in otherMatched and fact.CheckMatchBesidesID(fact2):
							otherMatched.append(fact2)
							toMatch-=1
							break
				return toMatch==0
		return False

	def __eq__(self, other):
		if isinstance(other, self.__class__):
			return set(self.replacementFacts)==set(other.replacementFacts)
		else:
			return False

	def __ne__(self, other):
		if isinstance(other, self.__class__):
			return not self.__eq__(other)
		else:
			return True

	def __hash__(self):
		return hash(tuple(["EmptyFact", self.replacementFacts]))

	def clone(self):
		fact = EmptyFact(self.replacementFacts)
		return fact

	def __str__(self):
		rpFactStr = ""
		for fact in self.replacementFacts:
			rpFactStr+=str(fact)+"|"
		return "EmptyFact: "+str(rpFactStr)

#TODO; hidden variable fact