using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Generator_list
{
    public class IsCorrect
    {
        private string[] splitedInputText, splitedCategoryNames;
        private int lineError = 1;
        private ListGenerator listGenerator;
        private bool lineIsWithoutError = true;
        public bool LineIsWithoutError { get { return lineIsWithoutError; } }

        public IsCorrect(ListGenerator listGenerator)
        {            
            this.listGenerator = listGenerator;            
        }

        public void CheckIfCorrect(string[] splitedInputText, string[] splitedCategoryNames)
        {
            UpdateClass(splitedInputText, splitedCategoryNames);
            int textsInLine = splitedInputText.Length;
            int namesInCategory = splitedCategoryNames.Length;
            int extraTextToJoin = AdditionalTextToJoin();


            if (extraTextToJoin > 0)
            {
                CheckWithExtraTextInCategory(textsInLine, namesInCategory, extraTextToJoin);
            }
            else
            {
                CheckIsNoExtraTextInCategory(textsInLine, namesInCategory);
            }

            lineError++;
        }

        private void UpdateClass(string[] splitedInputText, string[] splitedCategoryNames)
        {
            this.splitedInputText = splitedInputText;
            this.splitedCategoryNames = splitedCategoryNames;
        }

        private int AdditionalTextToJoin()
        {
            int additionalTextToJoin = 0;

            for (int i = 0; i < splitedCategoryNames.Length; i++)
            {
                if (listGenerator.ContainJoinTag(i))
                {
                    additionalTextToJoin += listGenerator.NumberOfCategoriesToJoin(i);
                }
                
            }

            return additionalTextToJoin;
        }

        private void CheckWithExtraTextInCategory(int textsInLine, int namesInCategory, int extraTextToJoin)
        {
            if (textsInLine > namesInCategory + extraTextToJoin)
            {
                MessageBox.Show("O " + (textsInLine - namesInCategory) + " wyrażeń więcej w linii: " + lineError);
                lineIsWithoutError = false;
            }
            else if (textsInLine < namesInCategory)
            {
                MessageBox.Show("O " + (namesInCategory - textsInLine) + " wyrażeń mniej w linii: " + lineError);
                lineIsWithoutError = false;
            }
        }

        private void CheckIsNoExtraTextInCategory(int textsInLine, int namesInCategory)
        {
            if (textsInLine > namesInCategory)
            {
                MessageBox.Show("O " + (textsInLine - namesInCategory) + " wyrażeń więcej w linii: " + lineError);
                lineIsWithoutError = false;
            }
            else if (textsInLine < namesInCategory)
            {
                MessageBox.Show("O " + (namesInCategory - textsInLine) + " wyrażeń mniej w linii: " + lineError);
                lineIsWithoutError = false;
            }
        }
    }
}
