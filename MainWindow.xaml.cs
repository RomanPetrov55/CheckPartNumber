using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TS = Tekla.Structures;
using TSM = Tekla.Structures.Model;
using TSF = Tekla.Structures.Filtering;
using TSFC = Tekla.Structures.Filtering.Categories;
using System.Collections;

namespace CheckPartNumber
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TSM.Model model;
        public string attWrite;
        public string attCheck;
        public string attNumCheck;
        public MainWindow()
        {
            
            InitializeComponent();
        }

        private void Button_Click_WriteNumbers(object sender, RoutedEventArgs e)
        {
            model = model ?? new TSM.Model();

            if (!model.GetConnectionStatus())
            {
                MessageBox.Show("Не удалось подключиться к Tekla Structures");
            }
            else
            {
                try
                {
                    attWrite = AttWrite.Text;

                    ProgressWindow progressWindow = new ProgressWindow();
                    progressWindow.Width = 450;
                    progressWindow.ProgressText.Text = "Выполняется запись номеров деталей в пользовательский атрибут";
                    progressWindow.Show();

                    //Получаем сборки в модели Tekla
                    ///Фильтруем из всей модели сборки
                    TSFC.ObjectFilterExpressions.Type objectType = new TSFC.ObjectFilterExpressions.Type(); ///целевой объект сравнения
                    TSF.NumericConstantFilterExpression type = new TSF.NumericConstantFilterExpression(TS.TeklaStructuresDatabaseTypeEnum.ASSEMBLY); ///объект, который ищем
                    var expression = new TSF.BinaryFilterExpression(objectType, TSF.NumericOperatorType.IS_EQUAL, type);
                    ///Создаем коллекцию
                    TSF.BinaryFilterExpressionCollection filterCollection = new TSF.BinaryFilterExpressionCollection
                    {
                        new TSF.BinaryFilterExpressionItem(expression)
                    };

                    ///Получаем enum сборок -> list сборок
                    TSM.ModelObjectEnumerator.AutoFetch = true;
                    var result = model.GetModelObjectSelector().GetObjectsByFilter(filterCollection).ToTeklaObjectList<TSM.Assembly>();

                    for (int i = 0; i < result.Count; i++)
                    {
                        string mainPartNumber = String.Empty;

                        //Объект сборки Tekla
                        TSM.Assembly currentAssembly = result[i];

                        //Главная деталь сборки
                        TSM.Part mainPart = currentAssembly.GetMainPart() as TSM.Part;

                        //Массив второстепенных деталей в сборке
                        ArrayList secondaryParts = currentAssembly.GetSecondaries();

                        //Номер главной детали
                        mainPart.GetReportProperty("PART_POS", ref mainPartNumber);
                        mainPart.SetUserProperty(attWrite, "");
                        mainPart.SetUserProperty(attWrite, mainPartNumber);

                        //Номера второстепенных деталей
                        for (int j = 0; j < secondaryParts.Count; j++)
                        {
                            string secPartNumber = String.Empty;
                            TSM.Part secondaryPart = (TSM.Part)secondaryParts[j];
                            secondaryPart.GetReportProperty("PART_POS", ref secPartNumber);
                            secondaryPart.SetUserProperty(attWrite, "");
                            secondaryPart.SetUserProperty(attWrite, secPartNumber);
                        }
                    }

                    model.CommitChanges();
                    progressWindow.Close();

                    MessageBox.Show("Готово! Текущие номера деталей записаны в пользовательский атрибут");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                } 
            }
        }

        private void Button_Click_CheckNumbers(object sender, RoutedEventArgs e)
        {
            model = model ?? new TSM.Model();

            if (!model.GetConnectionStatus())
            {
                MessageBox.Show("Не удалось подключиться к Tekla Structures");
            }
            else
            {
                try
                {
                    attNumCheck = AttNumCheck.Text;
                    attCheck = AttCheck.Text;

                    ProgressWindow progressWindow = new ProgressWindow();
                    progressWindow.Width = 350;
                    progressWindow.ProgressText.Text = "Выполняется проверка номеров деталей";
                    progressWindow.Show();

                    //Получаем сборки в модели Tekla
                    ///Фильтруем из всей модели сборки
                    TSFC.ObjectFilterExpressions.Type objectType = new TSFC.ObjectFilterExpressions.Type(); ///целевой объект сравнения
                    TSF.NumericConstantFilterExpression type = new TSF.NumericConstantFilterExpression(TS.TeklaStructuresDatabaseTypeEnum.ASSEMBLY); ///объект, который ищем
                    var expression = new TSF.BinaryFilterExpression(objectType, TSF.NumericOperatorType.IS_EQUAL, type);
                    ///Создаем коллекцию
                    TSF.BinaryFilterExpressionCollection filterCollection = new TSF.BinaryFilterExpressionCollection
                    {
                        new TSF.BinaryFilterExpressionItem(expression)
                    };

                    ///Получаем enum сборок -> list сборок
                    TSM.ModelObjectEnumerator.AutoFetch = true;
                    var result = model.GetModelObjectSelector().GetObjectsByFilter(filterCollection).ToTeklaObjectList<TSM.Assembly>();

                    for (int i = 0; i < result.Count; i++)
                    {
                        string mainPartNumber = String.Empty;

                        string mainPartAttNum = String.Empty;
                        string mainPartAttCheck = String.Empty;

                        //Объект сборки Tekla
                        TSM.Assembly currentAssembly = result[i];

                        //Главная деталь сборки
                        TSM.Part mainPart = currentAssembly.GetMainPart() as TSM.Part;

                        //Массив второстепенных деталей в сборке
                        ArrayList secondaryParts = currentAssembly.GetSecondaries();

                        //Номер главной детали
                        mainPart.GetReportProperty("PART_POS", ref mainPartNumber);

                        mainPart.GetUserProperty(attNumCheck, ref mainPartAttNum);
                        mainPart.SetUserProperty(attCheck, "");
                        
                        if (mainPartNumber == mainPartAttNum)
                        {
                            mainPart.SetUserProperty(attCheck, "0");
                        }
                        else
                        {
                            mainPart.SetUserProperty(attCheck, "1");
                        }

                        //Номера второстепенных деталей
                        for (int j = 0; j < secondaryParts.Count; j++)
                        {
                            string secPartNumber = String.Empty;

                            string secPartAttNum = String.Empty;
                            string secPartAttCheck = String.Empty;

                            TSM.Part secondaryPart = (TSM.Part)secondaryParts[j];

                            secondaryPart.GetReportProperty("PART_POS", ref secPartNumber);

                            secondaryPart.GetUserProperty(attNumCheck, ref secPartAttNum);
                            secondaryPart.SetUserProperty(attCheck, "");

                            if(secPartNumber == secPartAttNum)
                            {
                                secondaryPart.SetUserProperty(attCheck, "0");
                            }
                            else
                            {
                                secondaryPart.SetUserProperty (attCheck, "1");
                            }

                        }
                    }

                    model.CommitChanges();
                    progressWindow.Close();

                    MessageBox.Show("Готово! Выполнена проверка номеров деталей");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
