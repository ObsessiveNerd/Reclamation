using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace PygmyMonkey.GifCreator
{
    [ExecuteInEditMode]
    public class CustomRectPreview : MonoBehaviour
    {
        public static CustomRectPreview Singleton;

        private Material lineMaterial;
        private RecorderParameters parameters;

        private float startX;
        private float startY;
        private float endX;
        private float endY;

        private bool show = false;

        public void OnEnable()
        {
            Singleton = this;
        }

        private void createLineMaterial()
        {
            // Unity has a built-in shader that is useful for drawing simple colored things.
            lineMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        }

        public void Show(RecorderParameters parameters)
        {
            this.parameters = parameters;

            show = true;
            repaintGameView();
        }

        public void Hide()
        {
            show = false;
            repaintGameView();
        }

        // Will be called after all regular rendering is done
        void OnRenderObject()
        {
            if (!show)
            {
                return;
            }

            if (lineMaterial == null)
            {
                createLineMaterial();
            }

            if (parameters.section == RecorderParameters.Section.CustomRectAbsolute)
            {
                startX = parameters.absolutePosX / parameters.getMainGameViewSize().x;
                startY = parameters.absolutePosY / parameters.getMainGameViewSize().y;
                endX = (parameters.absolutePosX + parameters.absoluteWidth) / parameters.getMainGameViewSize().x;
                endY = (parameters.absolutePosY + parameters.absoluteHeight) / parameters.getMainGameViewSize().y;
            }
            else if (parameters.section == RecorderParameters.Section.CustomRectRelative)
            {
                startX = parameters.relativePosX;
                startY = parameters.relativePosY;
                endX = parameters.relativePosX + parameters.relativeWidth;
                endY = parameters.relativePosY + parameters.relativeHeight;
            }

            GL.PushMatrix();
            {
                lineMaterial.SetPass(0);

                GL.LoadOrtho();
                GL.Begin(GL.QUADS);
                {
                    GL.Color(parameters.customRectPreviewColor);
                    GL.Vertex3(startX, startY, 0);
                    GL.Vertex3(startX, endY, 0);
                    GL.Vertex3(endX, endY, 0);
                    GL.Vertex3(endX, startY, 0);
                }
                GL.End();
            }
            GL.PopMatrix();
        }

        private void repaintGameView()
        {
#if UNITY_EDITOR
            Type gameViewTypee = GetGameViewType();
            EditorWindow gameViewWindow = GetGameViewWindow(gameViewTypee);
            if (gameViewWindow != null)
            {
                gameViewWindow.Repaint();
            }
#endif
        }

#if UNITY_EDITOR
        private static Type GetGameViewType()
        {
            Assembly unityEditorAssembly = typeof(EditorWindow).Assembly;
            return unityEditorAssembly.GetType("UnityEditor.GameView");
        }

        private static EditorWindow GetGameViewWindow(Type gameViewType)
        {
            UnityEngine.Object[] obj = Resources.FindObjectsOfTypeAll(gameViewType);
            if (obj.Length > 0)
            {
                return obj[0] as EditorWindow;
            }

            return null;
        }
#endif

    }
}