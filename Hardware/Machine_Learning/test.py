import pandas as pd
import os

# script_dir = os.path.dirname(os.path.abspath(__file__))
# os.chdir(script_dir)
# print("Current working directory:", os.getcwd())
df = pd.read_csv('Hardware/Data_Extracting/training_data.csv')
df.head()