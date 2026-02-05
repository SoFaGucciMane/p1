//using UnityEngine;
//using UnityEditor;

//[CustomEditor(typeof(BoardService))]
//public class BoardServiceEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        BoardService board = (BoardService)target;

//        if (board.boardLayout != null && board.boardLayout.rows != null)
//        {
//            EditorGUILayout.LabelField("Board Layout");

//            for (int y = 0; y < board.boardLayout.rows.Length; y++)
//            {
//                EditorGUILayout.BeginHorizontal();

//                if (board.boardLayout.rows[y].row != null)
//                {
//                    for (int x = 0; x < board.boardLayout.rows[y].row.Length; x++)
//                    {
//                        board.boardLayout.rows[y].row[x] = EditorGUILayout.Toggle(board.boardLayout.rows[y].row[x], GUILayout.Width(20));
//                    }
//                }

//                EditorGUILayout.EndHorizontal();
//            }

//            EditorGUILayout.Space();
//        }

//        DrawDefaultInspector();

//        if (GUI.changed)
//        {
//            EditorUtility.SetDirty(target);
//        }
//    }
//}