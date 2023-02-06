using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Prism.Commands;
using System;
using System.Collections.Generic;

namespace CreateWorksheets
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;
        private Document _doc;

        //свойства___
        public List<FamilySymbol> SheetType { get; }
        public List<View> Views { get; }
        public int SheetsCount { get; set; }
        public string DesignedBy { get; set; }
        public DelegateCommand CreateSheets { get; }
        public FamilySymbol IsSelectedType { get; set; }    //выбранный тип основных надписей
        public View IsSelectedViews { get; set; }           //выбранный вид

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            _doc = _commandData.Application.ActiveUIDocument.Document;

            SheetType = SheetUtils.GetAllSheets(commandData);   //типы основных надписей
            Views = ViewsUtils.GetViews(commandData);           //выбор вида
            SheetsCount = 1;                                    //кол-во листов
            DesignedBy = null;                                  //Разработал:
            CreateSheets = new DelegateCommand(OnCreateSheets); //создание листов
        }

        private void OnCreateSheets()                           //метод создания листов
        {
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            using (var ts = new Transaction(doc, "Create sheet"))
            {

                ts.Start();

                try
                {

                    for (int i = 0; i < SheetsCount; i++)
                    {
                        ViewSheet viewSheet = ViewSheet.Create(doc, IsSelectedType.Id);                             // создание листа
                        viewSheet.get_Parameter(BuiltInParameter.SHEET_DESIGNED_BY).Set(DesignedBy);                // вставка значения в графу "Разработал"

                        UV location = new UV((viewSheet.Outline.Max.U - viewSheet.Outline.Min.U) / 2,               //центр листа
                                          (viewSheet.Outline.Max.V - viewSheet.Outline.Min.V) / 2);

                        var viewDuplicated = IsSelectedViews.Duplicate(ViewDuplicateOption.Duplicate);

                        Viewport.Create(doc, viewSheet.Id, viewDuplicated, new XYZ(location.U, location.V, 0));     //добавление вида в лист
                    }
                }
                catch
                {

                }

                ts.Commit();

            }
            RaiseCloseRequest();
        }

        public event EventHandler CloseRequest;

        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}
