using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;

using Sq1.Core;

namespace Sq1.Widgets.Exceptions {
	public partial class ExceptionsControl {
		List<Exception>	exceptionsSelectedInTree { get {
			List<Exception> ret = new List<Exception>();
			IList mustBeExceptions = this.olvTreeExceptions.SelectedObjects;
			foreach (object mustBeException in mustBeExceptions) {
				Exception ex = mustBeException as Exception;
				if (ex == null) {
					string msg = "mustBeException[" + mustBeException  + "]";
					Assembler.PopupException(msg);
					continue;
				}
				ret.Add(ex);
			}
			return ret;
		} }

		string exceptionsSelectedInTree_AsString { get {
			string ret = "";
			foreach (Exception each in this.exceptionsSelectedInTree) {
				ret += this.exception_AsString(each);
			}
			return ret;
		} }

		string exception_AsString(Exception exceptionSingleSelectedInTree) {
			if (exceptionSingleSelectedInTree == null) return "";

			StringBuilder formattedException = new StringBuilder();
			formattedException.Append("EXCEPTION INFORMATION").Append(Environment.NewLine)
				.Append(Environment.NewLine)
				.Append("Date/Time: ").Append(DateTime.Now.ToString("F", CultureInfo.CurrentCulture))
				.Append(Environment.NewLine)
				.Append("Type: ").Append(exceptionSingleSelectedInTree.GetType().FullName).Append(Environment.NewLine)
				.Append("Message: ").Append(exceptionSingleSelectedInTree.Message).Append(Environment.NewLine)
				.Append("Source: ").Append(exceptionSingleSelectedInTree.Source).Append(Environment.NewLine)
				.Append("Target Method: ");
			if (exceptionSingleSelectedInTree.TargetSite != null) {
				formattedException.Append(exceptionSingleSelectedInTree.TargetSite.ToString());
			}
			formattedException.Append(Environment.NewLine).Append(Environment.NewLine)
				.Append("Call Stack:").Append(Environment.NewLine);

			StackTrace exceptionStack = new StackTrace(exceptionSingleSelectedInTree);

			for (int i = 0; i < exceptionStack.FrameCount; i++) {
				StackFrame exceptionFrame = exceptionStack.GetFrame(i);

				formattedException.Append("\t").Append("Method Name: ").Append(exceptionFrame.GetMethod().ToString()).Append(Environment.NewLine)
					.Append("\t").Append("\t").Append("File Name: ").Append(exceptionFrame.GetFileName()).Append(Environment.NewLine)
					.Append("\t").Append("\t").Append("Column: ").Append(exceptionFrame.GetFileColumnNumber()).Append(Environment.NewLine)
					.Append("\t").Append("\t").Append("Line: ").Append(exceptionFrame.GetFileLineNumber()).Append(Environment.NewLine)
					.Append("\t").Append("\t").Append("CIL Offset: ").Append(exceptionFrame.GetILOffset()).Append(Environment.NewLine)
					.Append("\t").Append("\t").Append("Native Offset: ").Append(exceptionFrame.GetNativeOffset()).Append(Environment.NewLine)
					.Append(Environment.NewLine);
			}

			formattedException.Append("Inner Exception(s)").Append(Environment.NewLine);

			Exception innerException = exceptionSingleSelectedInTree.InnerException;

			while (innerException != null) {
				formattedException.Append("\t").Append("Exception: ")
					.Append(innerException.GetType().FullName).Append(Environment.NewLine);
				innerException = innerException.InnerException;
			}

			formattedException.Append(Environment.NewLine).Append("Custom Properties")
				.Append(Environment.NewLine);

			Type exceptionType = typeof(Exception);

			foreach (PropertyInfo propertyInfo in exceptionSingleSelectedInTree.GetType().GetProperties()) {
				if (exceptionType.GetProperty(propertyInfo.Name) == null) {
					formattedException.Append("\t").Append(propertyInfo.Name).Append(": ")
						.Append(propertyInfo.GetValue(exceptionSingleSelectedInTree, null))
						.Append(Environment.NewLine);
				}
			}

			return formattedException.ToString();
		}
		void copyExceptionsSelected_toClipboard() {
			Clipboard.SetDataObject(this.exceptionsSelectedInTree_AsString, true);
		}
	}
}