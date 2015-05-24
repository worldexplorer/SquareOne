using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sq1.Core.Support {
	//http://stackoverflow.com/questions/457676/check-if-a-class-is-derived-from-a-generic-class
	public static class ReflexionExtension {
		public static bool IsSubClassOfGeneric(this Type child, Type parent) {
			if (child == parent)
				return false;

			if (child.IsSubclassOf(parent))
				return true;

			var parameters = parent.GetGenericArguments();
			var isParameterLessGeneric = !(parameters != null && parameters.Length > 0 &&
				((parameters[0].Attributes & TypeAttributes.BeforeFieldInit) == TypeAttributes.BeforeFieldInit));

			while (child != null && child != typeof(object)) {
				var cur = GetFullTypeDefinition(child);
				if (parent == cur || (isParameterLessGeneric && cur.GetInterfaces().Select(i => GetFullTypeDefinition(i)).Contains(GetFullTypeDefinition(parent))))
					return true;
				else if (!isParameterLessGeneric)
					if (GetFullTypeDefinition(parent) == cur && !cur.IsInterface) {
						if (VerifyGenericArguments(GetFullTypeDefinition(parent), cur))
							if (VerifyGenericArguments(parent, child))
								return true;
					} else
						foreach (var item in child.GetInterfaces().Where(i => GetFullTypeDefinition(parent) == GetFullTypeDefinition(i)))
							if (VerifyGenericArguments(parent, item))
								return true;

				child = child.BaseType;
			}

			return false;
		}

		private static Type GetFullTypeDefinition(Type type) {
			return type.IsGenericType ? type.GetGenericTypeDefinition() : type;
		}

		private static bool VerifyGenericArguments(Type parent, Type child) {
			Type[] childArguments = child.GetGenericArguments();
			Type[] parentArguments = parent.GetGenericArguments();
			if (childArguments.Length == parentArguments.Length)
				for (int i = 0; i < childArguments.Length; i++)
					if (childArguments[i].Assembly != parentArguments[i].Assembly || childArguments[i].Name != parentArguments[i].Name || childArguments[i].Namespace != parentArguments[i].Namespace)
						if (!childArguments[i].IsSubclassOf(parentArguments[i]))
							return false;

			return true;
		}
	}
}
