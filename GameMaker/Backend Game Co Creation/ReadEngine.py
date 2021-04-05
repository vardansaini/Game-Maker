import numpy as np 
from state import *
from engine import *
import random
import pickle
import csv, sys

thisDirectory = sys.path[0]
splits = thisDirectory.split("/")
temp = ""
# String processing to get the path of current game
for i in range(0, len(splits)-1):	
	temp+=""+splits[i]+"/"
temp+="Assets/StreamingAssets/Frames/"
with open(temp+'LoadedGame.txt','r') as f:
	gameDirectory = f.read()
print()
print("GAME NAME IS:", gameDirectory)
print()

loading_rules = "./finalLearnedEngine_" + gameDirectory + ".p"
engine = pickle.load(open(loading_rules, "rb"))

ruleNum = 0
for rule in engine.rules:
	print ("RULE: "+str(ruleNum)+" "+str(rule.preEffect)+"->"+str(rule.postEffect))
	ruleNum+=1
	for cond in rule.conditions:
		print ( "    "+str(cond))