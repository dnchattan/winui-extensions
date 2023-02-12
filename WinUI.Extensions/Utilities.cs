using System.Diagnostics.CodeAnalysis;

namespace WinUI.Extensions
{
	internal static class Utilities
	{
		[return: NotNull]
		public static T AssertDefined<T>([NotNull] this T? value) => value ?? throw new ArgumentNullException(nameof(value));
	}
}
