import numpy as np 
from state import *
from engine import *
import random
import pickle

engine = pickle.load(open("partialLearnedEngine.p", "rb"))

ruleNum = 0
for rule in engine.rules:
	print ("RULE: "+str(ruleNum)+" "+str(rule.preEffect)+"->"+str(rule.postEffect))
	ruleNum+=1
	for cond in rule.conditions:
		print ( "    "+str(cond))