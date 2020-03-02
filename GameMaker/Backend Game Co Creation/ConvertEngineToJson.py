import numpy as np 
from state import *
from engine import *
import random
import pickle
import json

engine = pickle.load(open("engine-refineonly.p", "rb"))

ruleNum = 0
data = []
for rule in engine.rules:
	
	data.append({
    'type': '1',
    'fact': str(rule.preEffect),
    'id': ruleNum
	})

	data.append({
    'type': '2',
    'fact': str(rule.postEffect),
    'id': ruleNum
	})

	for cond in rule.conditions:
		#print ( "    "+str(cond))
		data.append({
	    'type': '0',
	    'fact': str(cond),
	    'id': ruleNum
		})

	ruleNum+=1

with open('data.json', 'w') as outfile:
    json.dump(data, outfile)