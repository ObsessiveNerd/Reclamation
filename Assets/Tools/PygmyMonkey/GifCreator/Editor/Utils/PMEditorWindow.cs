using UnityEditor;
using UnityEngine;

namespace PygmyMonkey.GifCreator.Utils
{
	public abstract class PMEditorWindow : EditorWindow
	{
		protected static EditorWindow createWindow<T>(string productName) where T : EditorWindow
		{
			T window = EditorWindow.GetWindow<T>(false);
			window.titleContent = new GUIContent(" " + getShortProductName(productName), (Texture2D)AssetDatabase.LoadAssetAtPath(getIconPath(productName), typeof(Texture2D)));
			return window;
		}

		public abstract string getProductName();
		public abstract string getVersionName();
		public abstract string getAssetStoreID();

		private static string getIconPath(string productName)
		{
			return "Assets/PygmyMonkey/" + getFormattedProductName(productName) + "/Editor/Textures/Icon.png";
		}

		protected static string getFormattedProductName(string productName)
		{
			return productName.Replace(" ", string.Empty).Trim();
		}

		private static string getShortProductName(string productName)
		{
			if (productName.Length <= 13)
			{
				return productName;
			}

			return productName.Substring(0, 13) + ".";
		}

		public abstract void init();

		private void OnEnable()
		{
			hideFlags = HideFlags.HideAndDontSave;

			init();
		}
		
		private void OnFocus()
		{
			Repaint();
		}
		
		private void OnSelectionChange()
		{
			Repaint();
		}

		private Vector2 scrollPosition;
		public virtual void drawBegin() {}
		public abstract void drawContent();
		public virtual void drawEnd() {}

		protected bool useCustomGUI = false;
		public virtual void drawCustomGUI() {}

		private void OnGUI()
		{
			if (useCustomGUI)
			{
				drawCustomGUI();
				return;
			}

			drawBegin();

			using (new GUIUtils.GUIScrollView(ref scrollPosition))
			{
				using (new GUIUtils.GUIHorizontal())
				{
					GUILayout.Space(5f);
					using (new GUIUtils.GUIVertical())
					{
						GUILayout.Space(5f);
						
						EditorGUILayout.LabelField(getProductName() + " - Version " + getVersionName(), LegacyGUIStyle.LabelVersionStyle);

						drawContent();

						GUILayout.Space(5f);
						PMUtils.DrawAskToRate(getFormattedProductName(getProductName()).ToLower() + "_show_rate_popup", getProductName(), getAssetStoreID());
						GUILayout.Space(10f);
					}
					GUILayout.Space(5f);
				}
			}

			drawEnd();
		}
	}
}