using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

//This is meant to open a TaskDialogue everytime someone clicks on the Import button
//There are no error but it's still doesn't seem to work. 

namespace WWRevitCommands
{
    [Transaction(TransactionMode.Manual)]
public class ImportWarningCommand : IExternalApplication
{
        ////This gives the option to the user but When I click yes and it proceeds with the import command, 
        ///next time I click on the import Cad button, it doesn't show the dialogue anymore until Revit restarts
        private AddInCommandBinding importBinding;

        public Result OnStartup(UIControlledApplication application)
        {
            // Get the command id for the Import CAD command
            RevitCommandId importCommandId = RevitCommandId.LookupCommandId("ID_FILE_IMPORT");

            // Create an add-in command binding for the Import CAD command
            importBinding = application.CreateAddInCommandBinding(importCommandId);

            // Subscribe to the Executed event of the Import CAD command
            importBinding.Executed += ImportCommand_Executed;

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            // Unsubscribe from the Executed event during shutdown
            if (importBinding != null)
            {
                importBinding.Executed -= ImportCommand_Executed;
            }

            return Result.Succeeded;
        }

        private void ImportCommand_Executed(object sender, ExecutedEventArgs e)
        {
            UIApplication uiapp = sender as UIApplication;

            TaskDialog tDialog = new TaskDialog("Import Warning");
            tDialog.MainContent = "Do you want to proceed with the import?";
            tDialog.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No;

            if (tDialog.Show() == TaskDialogResult.Yes)
            {
                // Unsubscribe from the Executed event to prevent recursion
                importBinding.Executed -= ImportCommand_Executed;

                // Invoke the default import command
                uiapp.PostCommand(RevitCommandId.LookupCommandId("ID_FILE_IMPORT"));
            }
            else
            {
                TaskDialog.Show("Cancel Import", "Import cancelled");
            }
        }
    }
}

 