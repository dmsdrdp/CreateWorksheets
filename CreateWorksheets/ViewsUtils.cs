using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateWorksheets
{
    public class ViewsUtils
    {
        public static List<View> GetViews(ExternalCommandData commandData)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            return new FilteredElementCollector(doc)
                .OfClass(typeof(View))
                .Cast<View>()
                .ToList();
        }

    }
}
