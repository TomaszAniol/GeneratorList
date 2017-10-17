using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Generator_list
{
    public class ListGenerator
    {
        public ListGenerator(TextBox inputTextBox)
        {
            this.inputTextBox = inputTextBox;
            
        }

        public void UpdateList(string className, string listName, string categoryName)
        {
            this.className = className;
            this.listName = listName;
            this.categoryName = categoryName;
        }
        
        private TextBox inputTextBox;
        private string className, listName, categoryName, listTextWriter;
        private string[] splitLines, splitedInputText, splitedCategoryNames;
        private int synchronizedIndexCategoryWithName, numberOfCategoriesToJoin, breakIndex;
        private List<SplitLinesList> listAfterAscending;

        private IsCorrect correctChecking;

        public string ListTextGenerator { get { return listTextWriter; } }

        public void CreateList()
        {
            correctChecking = new IsCorrect(this); 
            SplitInputTextLines();
            SplitCategoryNames();
            WriteFirstLineInGenerator();            
            SplitLinesToJoinCategoryNamesWithInputText();           
            
            string endOfList = " };";
            listTextWriter += endOfList;

            if (correctChecking.LineIsWithoutError)                        
               inputTextBox.Text = listTextWriter;
        }

        private void SplitInputTextLines()
        {
            splitLines = inputTextBox.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }

        private void SplitCategoryNames()
        {
            splitedCategoryNames = categoryName.Split(new char[] { ' ' });
        }

        private void WriteFirstLineInGenerator()
        {
            listTextWriter = "List<" + className + "> " + listName + " = new List<" + className + ">() {" + Environment.NewLine;
        }

        private void SplitLinesToJoinCategoryNamesWithInputText()
        {
            foreach (string splitLine in splitLines)
            {
                WriteLineInListOnFirstPosition();
                SplitInputTextFromLine(splitLine);

                synchronizedIndexCategoryWithName = 0;
                
                correctChecking.CheckIfCorrect(splitedInputText, splitedCategoryNames);

                if (correctChecking.LineIsWithoutError)
                {
                    JoinAllCategoryNamesWithInputText();
                }
            }
        }

        private void WriteLineInListOnFirstPosition()
        {
            listTextWriter += "new " + className + Environment.NewLine + " { ";
        }

        private void SplitInputTextFromLine(string splitLine)
        {
            splitedInputText = splitLine.Split(new char[] { ' ' });
        }

        private void JoinAllCategoryNamesWithInputText()
        {
            for (int i = 0; i < splitedCategoryNames.Length; i++)
            {
                JoinCategoryNameWithInputText(i);
            }
            string endOfListLine = "}," + Environment.NewLine;
            listTextWriter += endOfListLine;
        }

        private void JoinCategoryNameWithInputText(int i)
        {
            JoinNameWithText(i);
            int endOfCategory = splitedCategoryNames.Length - 1;

            if (i < endOfCategory)
            {
                listTextWriter += ", ";
            }
        }

        private void JoinNameWithText(int i)
        {
            bool joinTwoOrMoreTextWithOneCategory = ContainJoinTag(i);
            bool categoryIsNotString = splitedCategoryNames[i].Contains("`");

            if (joinTwoOrMoreTextWithOneCategory)
            {
                JoinMoreTextWithOneCategory(i);
            }
            else if (categoryIsNotString)
            {
                listTextWriter += WriteWithOutQuotationMark(i);
            }
            else
            {
                listTextWriter += WriteWithQuotationMark(i);
            }
            synchronizedIndexCategoryWithName++;
        }

        public bool ContainJoinTag(int i)
        {
            return splitedCategoryNames[i].Contains("`@");
        }

        private void JoinMoreTextWithOneCategory(int i)
        {
            numberOfCategoriesToJoin = NumberOfCategoriesToJoin(i);
            int joinCategoryIndex = numberOfCategoriesToJoin + synchronizedIndexCategoryWithName;            
            string joinCategoryText = TextInputJoinWithNextTextInput(joinCategoryIndex);
            WhenBreakInTextAppers(ref joinCategoryIndex, ref joinCategoryText);
            WriteJoinTextWithOtherText(i, joinCategoryText);

            joinCategoryIndex--;
            synchronizedIndexCategoryWithName = joinCategoryIndex;
        }

        public int NumberOfCategoriesToJoin(int i)
        {            
            return int.Parse(splitedCategoryNames[i].Substring(PositionOfMarker(i) + 2));
        }

        private int PositionOfMarker(int i)
        {
            return splitedCategoryNames[i].IndexOf("`@");
        }

        private string TextInputJoinWithNextTextInput(int joinCategoryIndex, int numberOfWhiles = 0)
        {
            string result = "";
            for (int p = synchronizedIndexCategoryWithName; p<joinCategoryIndex; p++)
            {
                numberOfWhiles++;
                result += " " + splitedInputText[p];
                bool endOfTextsToJoin = splitedInputText[p].Contains("`");
                if (endOfTextsToJoin)
                {                    
                    breakIndex += numberOfCategoriesToJoin - numberOfWhiles;
                    break;
                }
            }
            result = result.TrimStart(' ');
            return result;
        }

        private void WhenBreakInTextAppers(ref int joinCategoryIndex, ref string joinCategoryText)
        {
            if (breakIndex > 0)
            {
                joinCategoryIndex -= breakIndex;
                joinCategoryText = joinCategoryText.TrimEnd('`');
            }
        }

        private void WriteJoinTextWithOtherText(int i, string joinCategoryText)
        {
            listTextWriter += splitedCategoryNames[i].Remove(PositionOfMarker(i)) + " = \"" + joinCategoryText + "\"";
        }

        private string WriteWithOutQuotationMark(int i)
        {
            return splitedCategoryNames[i].TrimEnd('`') + " = " + splitedInputText[synchronizedIndexCategoryWithName];
        }

        private string WriteWithQuotationMark(int i)
        {
            return splitedCategoryNames[i] + " = \"" + splitedInputText[synchronizedIndexCategoryWithName] + "\"";
        }   
       

        //Test Area

        public void prepareList()
        {
            SplitInputTextLines();
            SplitCategoryNames();
            ListToPrepare listToMake = new ListToPrepare(splitLines);
            listAfterAscending = new List<SplitLinesList>();
            listAfterAscending.AddRange(listToMake.sortListByCount());
            inputTextBox.Text = "";           
            foreach (SplitLinesList line in listAfterAscending)
            {
                inputTextBox.Text += line.SplitLines + Environment.NewLine;
            }
            SplitInputTextLines();
            SplitCategoryNames();
        }

        
        public void compareLines()
        {
            string[] splitNewLines = inputTextBox.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            int listIndex = 0;
            int wordIndex = 0;
            List<string> sentenceList = new List<string>();
            List<string> newSentenceList = new List<string>();
            List<string> splitList = new List<string>(splitNewLines);
            string joinWords = "";

            for (int i = 0; i < splitNewLines.Length; i++)
            {
                if (!(splitNewLines[i] == splitLines[i]))
                {
                    listIndex = i;
                }
            }


                    for (int i = 0; i < 2; i++)
                    {
                int index = listIndex + i + 1;
                sentenceList.Add(splitNewLines[index]);
                    }

                    string[] splitWords = splitNewLines[listIndex].Split(new char[] { ' ' });

                    for (int id = 0; id < splitWords.Length; id++)
                    {
                        if (splitWords[id].Contains("`"))
                        {
                            wordIndex = id;
                        }
                    }

                    foreach (string sentence in sentenceList)
                    {
                joinWords = "";
                string[] splitWordsSentence = sentence.Split(new char[] { ' ' });
                        splitWordsSentence[wordIndex] += "`";
                        foreach (string joinWordsFromSentence in splitWordsSentence)
                        {
                            joinWords += joinWordsFromSentence + " ";
                        }
                        joinWords = joinWords.Trim();
                        newSentenceList.Add(joinWords);
                    }

                    

            for (int i = 0; i < 2; i++)
            {
                int index = listIndex + i + 1;
                splitList.RemoveAt(index);
                splitList.Insert(index, newSentenceList[i]);
            }

            foreach (string wordsInList in splitList)
            {
                inputTextBox.Text += wordsInList + Environment.NewLine;
            }


        }
    }
}
