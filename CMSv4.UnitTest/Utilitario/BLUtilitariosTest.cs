using Framework.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace CMSv4.UnitTest.Utilitario
{
    [TestClass]
    public class BLUtilitariosTest
    {

        [TestMethod]
        public void LerExcel()
        {
            string strExeFilePath = Assembly.GetExecutingAssembly().Location;
            string strWorkPath = Path.GetDirectoryName(strExeFilePath);

            var path = Path.Combine(strWorkPath, "FilesTeste", "gestao-custos-penalidades-template.xlsx");
            var exist = File.Exists(path);

            var data  = BLUtilitarios.LerExcel("Worksheet", path);

        }

    }
}
