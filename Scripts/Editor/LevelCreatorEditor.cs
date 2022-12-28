using PaperFold.Managers;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using FontStyle = UnityEngine.FontStyle;

namespace PaperFold.EditorScripts
{
    [CustomEditor(typeof(LevelCreator))]
    public class LevelCreatorEditor : Editor
    {
        LevelCreator levelCreator;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (levelCreator == null)
            {
                levelCreator = target as LevelCreator;
            }

            GUIStyle style = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fixedHeight = 25,
                margin = new RectOffset(5, 10, 10, 10),

                normal = new GUIStyleState()
                {
                    background = Texture2D.whiteTexture
                },

                active = new GUIStyleState()
                {
                    background = Texture2D.blackTexture
                }
            };

            GUIStyle style2 = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fixedHeight = 25,
                margin = new RectOffset(5, 10, 10, 10),

                normal = new GUIStyleState()
                {
                    background = Texture2D.whiteTexture
                },

                active = new GUIStyleState()
                {
                    background = Texture2D.blackTexture
                }
            };

            GUILayout.BeginHorizontal("box");

            if (GUILayout.Button("Create Level", style))
            {
                levelCreator.CreateLevel(false, true);

                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            }

            if (GUILayout.Button("Create Random Level", style))
            {
                levelCreator.CreateLevel(true, true);

                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            }

            //if (GUILayout.Button("Create All Level", style))
            //{
            //    levelCreator.CreateAllLevels();
            //    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            //}

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");

            if (GUILayout.Button("Delete", style2))
            {
                levelCreator.DeleteLevels();

                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            }

            if (GUILayout.Button("Delete All", style2))
            {
                levelCreator.DeleteAllLevels();

                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            }

            GUILayout.EndHorizontal();
        }
    }
}
