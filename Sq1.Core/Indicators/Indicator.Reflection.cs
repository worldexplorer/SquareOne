using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Sq1.Core.Indicators {
	public partial class Indicator {
				bool parametersByName_ReflectionForced;
				Dictionary<string, IndicatorParameter> parametersByName;
		public	Dictionary<string, IndicatorParameter> ParametersByName { get {
			if (parametersByName_ReflectionForced == false) return parametersByName;
			parametersByName_ReflectionForced = false;
			parametersByName.Clear();

			Type myChild = this.GetType();
			//v1
			//PropertyInfo[] lookingForIndicatorParameterProperties = myChild.GetProperties();
			//foreach (PropertyInfo indicatorParameterPropertyInfo in lookingForIndicatorParameterProperties) {
			//	Type expectingIndicatorParameterType = indicatorParameter.PropertyType;
			//v2
			//FieldInfo[] lookingForIndicatorParameterFields = myChild.GetFields();
			FieldInfo[] lookingForIndicatorParameterFields = myChild.GetFields(
															BindingFlags.Public
														| BindingFlags.NonPublic
														| BindingFlags.DeclaredOnly
														| BindingFlags.Instance
													);
			foreach (FieldInfo indicatorParameter in lookingForIndicatorParameterFields) {
				Type expectingIndicatorParameterType = indicatorParameter.FieldType;
				bool isIndicatorParameterChild = typeof(IndicatorParameter).IsAssignableFrom(expectingIndicatorParameterType);
				if (isIndicatorParameterChild == false) continue;
				//object expectingConstructedNonNull = indicatorParameter.GetValue(this, null);
				object expectingConstructedNonNull = indicatorParameter.GetValue(this);
				if (expectingConstructedNonNull == null) {
					string msg = "INDICATOR_DEVELOPER,INITIALIZE_INDICATOR_PARAMETER_IN_INDICATOR_CONSTRUCTOR Indicator[" + this.Name + "].ctor()"
						+ " { iParamFound = new new IndicatorParameter([" + indicatorParameter.Name + "], cur, min, max, increment); }";
					Assembler.PopupException(msg);
					continue;
				}
				IndicatorParameter indicatorParameterInstance = expectingConstructedNonNull as IndicatorParameter; 
				// NOPE_COZ_ATR.ParamPeriod=new IndicatorParameter("Period",..) indicatorParameterInstance.Name = indicatorParameterPropertyInfo.Name;
				indicatorParameterInstance.IndicatorName = this.Name;
				indicatorParameterInstance.ValidateSelf();
				parametersByName.Add(indicatorParameterInstance.FullName, indicatorParameterInstance);
			}

			// now Indicator.ToString() is referring to its own parameters...
			this.ParametersAsStringShort_forceRecalculate = true;
			string willGoToString = this.Parameters_asStringShort;
			foreach (IndicatorParameter indicatorParameterInstance in parametersByName.Values) {
				indicatorParameterInstance.Owner_asString = "indicatorParameterInstance REFLECTED_FOR[" + this.ToString() + "]";
			}
			return parametersByName;
		} }
		
		public		bool	ParametersAsStringShort_forceRecalculate;
					string	parametersAsStringShort_cached;
		public		string	Parameters_asStringShort					{ get {
			if (this.ParametersAsStringShort_forceRecalculate) {
				this.parametersAsStringShort_cached = null;
				this.ParametersAsStringShort_forceRecalculate = false;
			}
			if (parametersAsStringShort_cached == null) {
				StringBuilder sb = new StringBuilder();
				foreach (string paramName in this.ParametersByName.Keys) {
					IndicatorParameter param = this.ParametersByName[paramName];
					if (sb.Length > 0) sb.Append(",");
					sb.Append(paramName);
					sb.Append(":");
					string shortName = param.ValuesAsString;
					sb.Append(shortName);
				}
				this.parametersAsStringShort_cached = sb.ToString();
			}
			return this.parametersAsStringShort_cached;
		} }
		
		public		string	NameWithParameters { get {
			string parameters = this.Parameters_asStringShort;
			if (string.IsNullOrEmpty(parameters)) parameters = "NOT_BUILT_YET_ParametersByName_DIDNT_INVOKE_BuildParametersFromAttributes()";
			string ret = this.Name + " (" + parameters + ")";
			return ret;
		} }
	
	}
}
