using System;

namespace VM2.PageSpeed
{
    public abstract class BLPageSpeedUtilBase
    {
        public virtual string ColorGreen => "#16a085";

        public virtual string ColorYellow => "#f1c40f";

        public virtual string ColorRed => "#e74c3c";

        #region ScoreToHexaColor

        public virtual string ScoreToHexaColor(int score)
        {
            if (score >= 90)
                return ColorGreen;

            if (score >= 50)
                return ColorYellow;

            return ColorRed;
        }

        public virtual string ScoreToHexaColor(object score)
        {
            return ScoreToHexaColor(ConvertScoreToInt(score));
        }

        #endregion

        #region ConvertScoreToInt

        public virtual int ConvertScoreToInt(object score)
        {
            return Convert.ToInt16(Convert.ToDouble(score ?? 0) * 100);
        }

        #endregion

        #region BuildDescritionLinks

        public virtual string BuildDescritionLinks(string value)
        {
            if (!value.Contains("[") || !value.Contains("]"))
                return value;

            if (!value.Contains("(") || !value.Contains(")"))
                return value;

            var index = value.IndexOf("](");

            while (index != -1)
            {
                var startIndex = value.Substring(0, index).LastIndexOf("[");
                var endIndex = value.Substring(startIndex).IndexOf(")") + startIndex + 1;
                var lenghtOfTextToBeReplaced = endIndex - startIndex;

                var textToRemoveFromOriginalValue = value.Substring(startIndex, lenghtOfTextToBeReplaced);

                var aHref = textToRemoveFromOriginalValue.Split('(', ')')[1];
                var aText = textToRemoveFromOriginalValue.Split('[', ']')[1];

                var aElement = $"<a href=\"{aHref}\" target=\"_blanck\">{aText}</a>";

                value = value.Replace(textToRemoveFromOriginalValue, aElement);

                index = value.IndexOf("](");
            }

            return value;
        }

        #endregion
    }
}
