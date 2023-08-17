using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class DialogEditorWindow : EditorWindow
{
    /* DialogData dialogData;
    Vector2 scrollPos;

    Dictionary<QuestionData, bool> questionFoldouts = new Dictionary<QuestionData, bool>();
    Dictionary<AnswerData, bool> answerFoldouts = new Dictionary<AnswerData, bool>();

    [MenuItem("Window/Dialog Editor")]
    public static void ShowWindow()
    {
        GetWindow<DialogEditorWindow>("Dialog Editor");
    }

    void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        if (dialogData == null)
        {
            if (GUILayout.Button("Load JSON"))
            {
                string path = EditorUtility.OpenFilePanel("Load Dialog JSON", "", "json");
                if (!string.IsNullOrEmpty(path))
                {
                    string json = File.ReadAllText(path);
                    dialogData = JsonUtility.FromJson<DialogData>(json);

                    questionFoldouts.Clear();
                    answerFoldouts.Clear();

                    InitializeFoldouts(dialogData.dialogue);
                }
            }
        }
        else
        {
            dialogData.npcName = EditorGUILayout.TextField("NPC Name", dialogData.npcName);

            for (int i = 0; i < dialogData.dialogue.Count; i++)
            {
                bool foldout = false;
                questionFoldouts.TryGetValue(dialogData.dialogue[i], out foldout);

                foldout = EditorGUILayout.Foldout(foldout, "Question " + (i + 1));
                questionFoldouts[dialogData.dialogue[i]] = foldout;

                if (foldout)
                {
                    dialogData.dialogue[i] = DrawQuestion(dialogData.dialogue[i]);

                    if (GUILayout.Button("Remove Question"))
                    {
                        questionFoldouts.Remove(dialogData.dialogue[i]);
                        dialogData.dialogue.RemoveAt(i);
                        i--;
                    }

                    EditorGUILayout.Space();
                }
            }

            if (GUILayout.Button("Add Question"))
            {
                QuestionData newQuestion = new QuestionData { answers = new AnswerData[0] };
                dialogData.dialogue.Add(newQuestion);
                questionFoldouts[newQuestion] = true;
            }

            if (GUILayout.Button("Save JSON"))
            {
                string path = EditorUtility.SaveFilePanel("Save Dialog JSON", "", "dialog.json", "json");
                if (!string.IsNullOrEmpty(path))
                {
                    string json = JsonUtility.ToJson(dialogData, true);
                    File.WriteAllText(path, json);
                }
            }
        }

        EditorGUILayout.EndScrollView();
    }

    void InitializeFoldouts(List<QuestionData> questions)
    {
        foreach (var question in questions)
        {
            questionFoldouts[question] = false;

            foreach (var answer in question.answers)
            {
                answerFoldouts[answer] = false;
                if (answer.response != null && answer.response.Length > 0)
                {
                    InitializeFoldoutsRecursive(answer.response);
                }
            }
        }
    }

    void InitializeFoldoutsRecursive(QuestionData[] questions)
    {
        foreach (var question in questions)
        {
            questionFoldouts[question] = false;

            foreach (var answer in question.answers)
            {
                answerFoldouts[answer] = false;
                if (answer.response != null && answer.response.Length > 0)
                {
                    InitializeFoldoutsRecursive(answer.response);
                }
            }
        }
    }

    QuestionData DrawQuestion(QuestionData question)
{
    question.question = EditorGUILayout.TextField("Question", question.question);

    for (int i = 0; i < question.answers.Length; i++)
    {
        bool foldout = false;
        answerFoldouts.TryGetValue(question.answers[i], out foldout);

        foldout = EditorGUILayout.Foldout(foldout, "Answer " + (i + 1));
        answerFoldouts[question.answers[i]] = foldout;

        if (foldout)
        {
            question.answers[i] = DrawAnswer(question.answers[i], "Response " + (i + 1));

            if (GUILayout.Button("Remove Answer"))
            {
                answerFoldouts.Remove(question.answers[i]);
                var answersList = new List<AnswerData>(question.answers);
                answersList.RemoveAt(i);
                question.answers = answersList.ToArray();
                i--;
            }
        }
    }

    if (GUILayout.Button("Add Answer"))
    {
        AnswerData newAnswer = new AnswerData { response = new QuestionData[0] };
        var answersList = new List<AnswerData>(question.answers);
        answersList.Add(newAnswer);
        question.answers = answersList.ToArray();
        answerFoldouts[newAnswer] = true;
    }

    return question;
}


    AnswerData DrawAnswer(AnswerData answer, string label)
{
    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

    EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
    answer.answer = EditorGUILayout.TextField("Answer", answer.answer);

    if (answer.response != null && answer.response.Length > 0)
    {
        for (int i = 0; i < answer.response.Length; i++)
        {
            EditorGUILayout.Space();
            answer.response[i] = DrawQuestion(answer.response[i]);
        }
    }

    EditorGUILayout.BeginHorizontal();
    GUILayout.FlexibleSpace();
    if (GUILayout.Button("Add Nested Question"))
    {
        var questionsList = new List<QuestionData>(answer.response ?? new QuestionData[0]);
        questionsList.Add(new QuestionData { answers = new AnswerData[0] });
        answer.response = questionsList.ToArray();
    }
    EditorGUILayout.EndHorizontal();

    EditorGUILayout.EndVertical();

    return answer; 
} */



}