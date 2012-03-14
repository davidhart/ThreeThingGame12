using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace TTG
{
	//This is how we boost a mixed array :P
	//levels are 15*15
	struct LevelItem
	{
		public char[] Item;
		public int LineNumber; //mainly in for debugging purposes
	}
	
	public class Level
	{
		const int MAX = 15;
		LevelItem[] levelArray = new LevelItem[MAX];
		int lineCount = 0;
		
		//TODO add the items to be drawn
		GameObject3D[] levelObjects = new GameObject3D[MAX * MAX];
		
		public Level()
		{
			for (int i = 0; i < MAX; i++)
			{
				levelArray[i].Item = new char[MAX];
			}
		}
		
		void Load(string filename)
		{
			try
			{
				char [] line = new char [MAX];
				using (File.OpenText(filename))
				{
					lineCount++;
				}  
				TextReader tr = new StreamReader(filename);
				
				for(int i = 0; i < lineCount; ++lineCount)
				{
					//Reads in the current line and converts to char array
					levelArray[i].LineNumber = i;
					line = tr.ReadLine().ToCharArray();
					
					for(int j = 0; j < line.Length; ++j)
					{
						levelArray[i].Item[j] = line[j];
					}
				}
			}
			catch
			{
				throw new Exception("Cannot read level file");
			}
			
		}
		
		void Draw()
		{
			for(int i = 0; i < MAX; ++i)
			{
				for (int j = 0; j < MAX; ++j)
				{
					if (levelArray[i].Item[j] == '#') //Upper level
					{
					}
					if (levelArray[i].Item[j] == '@') //Penguin Path
					{
					}
					if (levelArray[i].Item[j] == '.') //Turret Emplacement
					{
					}
					if (levelArray[i].Item[j] == 'T') //Tank Start
					{
					}
					if (levelArray[i].Item[j] == 'P') //Penguin Start
					{
					}
				}
			}
		}
		
		void Update()
		{
		}
	}
}

