using System;
using System.Collections.Generic;

public static class CollectionExtensions
{

	/// コレクション(シーケンス)の各要素に対して指定処理を実行する
	/// @tparam		T		コレクションの要素の型
	/// @param[in]	source	コレクション
	/// @param[in]	action	各要素に対して実行する Action デリゲート
	public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
	{
		foreach (var item in source) action(item);
	}

	/// コレクション(シーケンス)の各要素に対してインデクスを付与して指定処理を実行する
	/// @tparam		T		コレクションの要素の型
	/// @param[in]	source	コレクション
	/// @param[in]	action	各要素に対して実行する Action デリゲート
	public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
	{
		int index = 0;
		foreach (var item in source) action(item, index++);
	}
}
