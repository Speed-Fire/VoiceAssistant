using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Plugin.Base.Extensions
{
	public static class JsonNodeExtensions
	{
		/// <summary>
		/// Retrieves nested node from root.
		/// </summary>
		/// <param name="root"></param>
		/// <param name="mainkey">Sequence of keys separated by '.' or ':'.</param>
		/// <param name="result">Needed node or null, if there is no value on the path.</param>
		/// <returns>True if path exist, otherwise - false.</returns>
		public static bool TryGetNestedNode(this JsonNode root, string mainkey, out JsonNode? result)
		{
			result = null;
			var keys = mainkey.Split(':');

			var tmp = root.AsObject();
			for(int i = 0; i < keys.Length; i++)
			{
				if(tmp is null)
					return false;

				if (!tmp.ContainsKey(keys[i]))
					return false;

				var node = tmp[keys[i]];

				if(node is not JsonObject)
				{
					if(i == keys.Length - 1)
					{
						result = node;
						return true;
					}
					else
					{
						return false;
					}
				}

				tmp = node?.AsObject();
			}

			result = tmp;

			return true;
		}

		public static void SetNestedValue(this JsonNode root, string mainkey, object value)
		{
			var keys = mainkey.Split(':');

			var tmp = root.AsObject();
			foreach(var key in keys)
			{
				if(!tmp.ContainsKey(key) || tmp[key] is not JsonObject)
					tmp[key] = new JsonObject();

				tmp = tmp[key]!.AsObject();
			}

			tmp.ReplaceWith(value);
		}
	}
}
