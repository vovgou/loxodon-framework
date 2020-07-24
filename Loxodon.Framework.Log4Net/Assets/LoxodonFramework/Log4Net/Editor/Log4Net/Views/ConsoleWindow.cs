using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using Loxodon.Log.Log4Net;
using System.Text;
using System.IO;
using UnityEditorInternal;

namespace Loxodon.Log.Editors.Log4Net
{

    public class ConsoleWindow : EditorWindow
    {
        [MenuItem("Tools/Loxodon/Log4Net Console")]
        static void ShowWindow()
        {
            var window = GetWindow<ConsoleWindow>(false, "Remoting");
            window.Show();
        }

        private ConsoleVM consoleVM;

        private Vector2 loggingPanelScrollPosition;
        private Vector2 detailPanelScrollPosition;
        private float loggingVerticalScrollBarPercent;

        private Texture[] levelIconTextures;
        private GUISwitchContentData playToggle;
        private GUISwitchContentData[] levelButtonDatas;

        private GUIContentData clearButton;
        private GUIContentData saveButton;

        private GUISwitchContentData collapseToggle;

        private GUISwitchContentData[] columnButtonDatas;

        private GUIStyle entryStyleBackEven;
        private GUIStyle entryStyleBackOdd;
        private GUIStyle detailStyle;
        private GUIStyle countBadgeStyle;

        private GUIStyle toolbarSeachTextFieldStyle;
        private GUIStyle toolbarSeachCancelButtonStyle;

        private float toolbarHeight = 20f;
        private float splitterRectHeight = 20f;
        private Rect verticalSplitterRect;
        private Rect verticalSplitterLineRect;
        private float verticalSplitterPercent;
        private bool resizingVerticalSplitter = false;

        [NonSerialized]
        private Texture2D splitterLineTexture;

        private float lineHeight;

        private int selectedIndex = -1;
        private double lastClickTime = 0f;
        private double doubleClickInterval = 0.2f;

        private List<LoggingData> renderedList;
        private List<int> renderedLineCountList;

        void OnEnable()
        {
            loggingVerticalScrollBarPercent = 1f;
            lineHeight = 30;
            toolbarHeight = 20f;
            splitterRectHeight = 10f;
            verticalSplitterPercent = 0.7f;          
            verticalSplitterLineRect = new Rect(0f, (position.height * verticalSplitterPercent), position.width, 1);
            verticalSplitterRect = new Rect(verticalSplitterLineRect.x, verticalSplitterLineRect.y - splitterRectHeight / 2f, verticalSplitterLineRect.width, splitterRectHeight);
            this.renderedList = new List<LoggingData>();
            this.renderedLineCountList = new List<int>();

            if (consoleVM == null)
                consoleVM = new ConsoleVM();
            consoleVM.OnEnable(); 
                       
        }

        void Disable()
        {
            if (consoleVM != null)
                consoleVM.OnDisable();
        }

        void OnDestroy()
        {
            if (this.consoleVM != null)
            {
                this.consoleVM.Stop();
                this.consoleVM = null;
            }
        }

        private void Init()
        {
            if (this.splitterLineTexture!=null)
                return;

            this.entryStyleBackEven = new GUIStyle("CN EntryBackEven");
            this.entryStyleBackEven.margin = new RectOffset(0, 0, 0, 0);
            this.entryStyleBackEven.border = new RectOffset(0, 0, 0, 0);
            this.entryStyleBackEven.fixedHeight = 0;
            this.entryStyleBackEven.fixedWidth = 0;
            this.entryStyleBackEven.richText = true;
            this.entryStyleBackEven.imagePosition = ImagePosition.ImageLeft;
            this.entryStyleBackEven.contentOffset = new Vector2(0f, 0f);
            this.entryStyleBackEven.padding = new RectOffset(10, 0, 0, 0);
            this.entryStyleBackEven.alignment = TextAnchor.MiddleLeft;

            this.entryStyleBackOdd = new GUIStyle("CN EntryBackOdd");
            this.entryStyleBackOdd.margin = new RectOffset(0, 0, 0, 0);
            this.entryStyleBackOdd.border = new RectOffset(0, 0, 0, 0);
            this.entryStyleBackOdd.fixedHeight = 0;
            this.entryStyleBackOdd.fixedWidth = 0;
            this.entryStyleBackOdd.richText = true;
            this.entryStyleBackOdd.imagePosition = ImagePosition.ImageLeft;
            this.entryStyleBackOdd.contentOffset = new Vector2(0f, 0f);
            this.entryStyleBackOdd.padding = new RectOffset(10, 0, 0, 0);
            this.entryStyleBackOdd.alignment = TextAnchor.MiddleLeft;

            this.splitterLineTexture = new Texture2D(1, 1);
            this.splitterLineTexture.SetPixel(0, 0, Color.black);
            this.splitterLineTexture.Apply();

            this.detailStyle = new GUIStyle("CN EntryBackOdd");
            this.detailStyle.margin = new RectOffset(0, 0, 0, 0);
            this.detailStyle.border = new RectOffset(5, 5, 5, 5);
            this.detailStyle.richText = true;
            this.detailStyle.contentOffset = new Vector2(0f, 0f);
            this.detailStyle.padding = new RectOffset(5, 5, 5, 5);
            this.detailStyle.alignment = TextAnchor.UpperLeft;
            this.detailStyle.fixedWidth = 0;
            this.detailStyle.wordWrap = true;

            this.countBadgeStyle = new GUIStyle("CN CountBadge");
            this.countBadgeStyle.margin = new RectOffset(0, 0, 0, 0);
            this.countBadgeStyle.border = new RectOffset(0, 0, 0, 0);
            this.countBadgeStyle.contentOffset = new Vector2(0f, 0f);
            this.countBadgeStyle.padding = new RectOffset(3, 3, 3, 3);
            this.countBadgeStyle.fixedHeight = 0f;
            this.countBadgeStyle.fixedWidth = 0f;
            this.countBadgeStyle.alignment = TextAnchor.MiddleCenter;

            if (this.toolbarSeachTextFieldStyle == null)
                this.toolbarSeachTextFieldStyle = new GUIStyle("ToolbarSeachTextField");

            if (this.toolbarSeachCancelButtonStyle == null)
                this.toolbarSeachCancelButtonStyle = new GUIStyle("ToolbarSeachCancelButton");

            playToggle = new GUISwitchContentData(false, new GUIContent(Resources.Load<Texture2D>("Icons/play-on"), "Start"), new GUIContent(Resources.Load<Texture2D>("Icons/play-off"), "Stop"), EditorStyles.toolbarButton);

            clearButton = new GUIContentData(new GUIContent(Resources.Load<Texture2D>("Icons/delete"), "Clear"), EditorStyles.toolbarButton);
            saveButton = new GUIContentData(new GUIContent(Resources.Load<Texture2D>("Icons/save"), "Save"), EditorStyles.toolbarButton);

            collapseToggle = new GUISwitchContentData(consoleVM.Collapse, new GUIContent("Collapse", "Collapse"), EditorStyles.toolbarButton);

            levelIconTextures = new Texture[]
            {  Resources.Load<Texture2D>("Icons/debug")  ,
                 Resources.Load<Texture2D>("Icons/info")  ,
                 Resources.Load<Texture2D>("Icons/warn")  ,
                 Resources.Load<Texture2D>("Icons/error")  ,
                 Resources.Load<Texture2D>("Icons/fatal")
            };

            levelButtonDatas = new GUISwitchContentData[5];
            levelButtonDatas[0] = new GUISwitchContentData(consoleVM.IsLevelShow(Level.DEBUG), new GUIContent(levelIconTextures[0], "Debug"), EditorStyles.toolbarButton);
            levelButtonDatas[1] = new GUISwitchContentData(consoleVM.IsLevelShow(Level.INFO), new GUIContent(levelIconTextures[1], "Info"), EditorStyles.toolbarButton);
            levelButtonDatas[2] = new GUISwitchContentData(consoleVM.IsLevelShow(Level.WARN), new GUIContent(levelIconTextures[2], "Warn"), EditorStyles.toolbarButton);
            levelButtonDatas[3] = new GUISwitchContentData(consoleVM.IsLevelShow(Level.ERROR), new GUIContent(levelIconTextures[3], "Error"), EditorStyles.toolbarButton);
            levelButtonDatas[4] = new GUISwitchContentData(consoleVM.IsLevelShow(Level.FATAL), new GUIContent(levelIconTextures[4], "Fatal"), EditorStyles.toolbarButton);


            columnButtonDatas = new GUISwitchContentData[3];
            columnButtonDatas[0] = new GUISwitchContentData(consoleVM.IsColumnShow(Columns.TimeStamp), new GUIContent("Time", "Time"), EditorStyles.toolbarButton);
            columnButtonDatas[1] = new GUISwitchContentData(consoleVM.IsColumnShow(Columns.Thread), new GUIContent("Thread", "Thread"), EditorStyles.toolbarButton);
            columnButtonDatas[2] = new GUISwitchContentData(consoleVM.IsColumnShow(Columns.Logger), new GUIContent("Logger", "Logger"), EditorStyles.toolbarButton);
        }   

        void OnGUI()
        {
            Init();

            LoggingContainer container = consoleVM.GetCurrentContainer();

            Color oldColor = GUI.backgroundColor;
            this.DrawToolbar(container);
            GUI.backgroundColor = oldColor;
            this.DrawVerticalSplitter();
            GUI.backgroundColor = oldColor;
            this.DrawLoggingGrid(container);
            GUI.backgroundColor = oldColor;
            this.DrawLoggingDetail();
            GUI.backgroundColor = oldColor;

            this.Repaint();
        }

        Texture GetTextureIcon(Level level)
        {
            switch (level)
            {
                case Level.DEBUG:
                    return this.levelIconTextures[0];
                case Level.INFO:
                    return this.levelIconTextures[1];
                case Level.WARN:
                    return this.levelIconTextures[2];
                case Level.ERROR:
                    return this.levelIconTextures[3];
                case Level.FATAL:
                    return this.levelIconTextures[4];
                default:
                    return this.levelIconTextures[0];
            }
        }

        string GetColorString(Level level)
        {
            switch (level)
            {
                case Level.DEBUG:
                    return "#b4b4b4";
                case Level.INFO:
                    return "#0097e5";
                case Level.WARN:
                    return "#c1c04c";
                case Level.ERROR:
                    return "#e58600";
                case Level.FATAL:
                    return "#c04e43";
                default:
                    return "#b4b4b4";
            }
        }

        private GUIContent GetLogLineGUIContent(LoggingData data)
        {
            StringBuilder buf = new StringBuilder();
            buf.AppendFormat("<color={0}>", GetColorString(data.Level));

            if (this.consoleVM.IsColumnShow(Columns.TimeStamp))
                buf.AppendFormat("{0:yyyy-MM-dd HH:mm:ss.fff}", data.TimeStamp);

            if (this.consoleVM.IsColumnShow(Columns.Thread))
                buf.AppendFormat(" Thread[{0}]", data.ThreadName);

            buf.AppendFormat(" {0}", data.Level.ToString());

            if (this.consoleVM.IsColumnShow(Columns.Logger))
                buf.AppendFormat(" {0}", data.LoggerName);

            buf.AppendFormat(" - {0}", data.Message);
            buf.Append("</color>");

            return new GUIContent(buf.ToString(), GetTextureIcon(data.Level));
        }

        private string GetLogDetailContent(LoggingData data)
        {
            StringBuilder buf = new StringBuilder();
            buf.AppendFormat("<color={0}>", GetColorString(data.Level));

            if (this.consoleVM.IsColumnShow(Columns.TimeStamp))
                buf.AppendFormat("{0:yyyy-MM-dd HH:mm:ss.fff}", data.TimeStamp);
            if (this.consoleVM.IsColumnShow(Columns.Thread))
                buf.AppendFormat(" Thread[{0}]", data.ThreadName);

            buf.AppendFormat(" {0}", data.Level.ToString());

            if (this.consoleVM.IsColumnShow(Columns.Logger))
                buf.AppendFormat(" {0}", data.LoggerName);

            buf.AppendFormat(" - {0}", data.Message);
            buf.Append("</color>");
            buf.Append("\r\n\r\n");

            if (data.LocationInfo != null && data.LocationInfo.StackFrames != null)
            {
                foreach (var frame in data.LocationInfo.StackFrames)
                {
                    buf.Append(frame.FullInfo.Replace(Directory.GetCurrentDirectory().ToString() + @"\", "")).Append("\r\n");
                }
            }

            return buf.ToString();
        }

        string[] GetTerminalInfoOptions()
        {
            List<string> list = new List<string>();
            foreach (var info in consoleVM.TerminalInfos)
            {
                list.Add(info.ToString());
            }
            return list.ToArray();
        }

        void DrawToolbar(LoggingContainer container)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (this.consoleVM.CurrentIndex >= 0)
                consoleVM.CurrentIndex = EditorGUILayout.Popup(consoleVM.CurrentIndex, GetTerminalInfoOptions(), EditorStyles.toolbarDropDown, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));

            GUILayout.Space(5f);

            this.collapseToggle.Value = consoleVM.Collapse;
            this.Toggle(this.collapseToggle, () =>
            {
                consoleVM.Collapse = this.collapseToggle.Value;
            });

            GUILayout.Space(5f);

            DrawToolbarColumnButtons(container);

            GUILayout.Space(5f);

            GUILayout.FlexibleSpace();

            GUI.SetNextControlName("searchTextField");
            consoleVM.FilterText = EditorGUILayout.TextField(consoleVM.FilterText, this.toolbarSeachTextFieldStyle, GUILayout.MinWidth(100), GUILayout.MaxWidth(500));
            if (GUILayout.Button("", this.toolbarSeachCancelButtonStyle))
            {
                consoleVM.FilterText = "";
                GUI.FocusControl("");
            }

            GUILayout.FlexibleSpace();

            GUILayout.Space(5f);

            this.playToggle.Value = this.consoleVM.PlayState;
            this.Toggle(this.playToggle, () =>
            {
                this.consoleVM.PlayState = this.playToggle.Value;
                if (this.playToggle.Value)
                    this.consoleVM.Start();
                else
                    this.consoleVM.Stop();

            });

            if (consoleVM.PlayState)
            {
                if (GUILayout.Button("Running", EditorStyles.toolbarDropDown, GUILayout.Width(60)))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent(string.Format("{0}:{1}", consoleVM.GetLocalIPAddress().ToString(), consoleVM.Port.ToString())), false, delegate ()
                    {
                    });
                    menu.ShowAsContext();
                }
            }
            else {
                consoleVM.Port = EditorGUILayout.IntField(consoleVM.Port, EditorStyles.toolbarTextField, GUILayout.Width(52));
            }

            GUILayout.Space(5f);

            this.DrawToolbarLevelButtons(container);


            if (this.Button(this.clearButton))
            {
                EditorApplication.delayCall += () => this.consoleVM.ClearLoggingData();
            }

            if (this.Button(this.saveButton))
            {
                EditorApplication.delayCall += () => this.consoleVM.SaveLoggingData();
            }
            GUILayout.Space(5f);
            EditorGUILayout.EndHorizontal();
        }

        void DrawToolbarLevelButtons(LoggingContainer container)
        {
            for (int i = 0; i < this.levelButtonDatas.Length; i++)
            {
                var data = levelButtonDatas[i];
                Level level = (Level)(i + 1);
                data.Value = consoleVM.IsLevelShow(level);
                int count = container.GetCount(level);
                data.Text = count < 1000 ? count.ToString() : "999+";
                this.Toggle(data, () =>
                {
                    consoleVM.SetLevelShow(level, data.Value);
                });
            }
        }

        void DrawToolbarColumnButtons(LoggingContainer container)
        {
            for (int i = 0; i < this.columnButtonDatas.Length; i++)
            {
                var data = columnButtonDatas[i];
                Columns column = (Columns)i;
                data.Value = consoleVM.IsColumnShow(column);
                this.Toggle(data, () =>
                {
                    consoleVM.SetColumnShow(column, data.Value);
                });
            }
        }

        bool ShouldShow(LoggingEntry logging)
        {
            if (!consoleVM.IsLevelShow(logging.Level))
                return false;

            if (string.IsNullOrEmpty(consoleVM.FilterText))
                return true;

            if (Regex.IsMatch(logging.Message, consoleVM.FilterText) || (consoleVM.IsColumnShow(Columns.Logger) && Regex.IsMatch(logging.LoggerName, consoleVM.FilterText)))
                return true;

            return false;
        }

        void DrawLoggingGrid(LoggingContainer container)
        {
            var areaRect = new Rect(0f, toolbarHeight, position.width, position.height * this.verticalSplitterPercent - toolbarHeight - splitterRectHeight / 2f);
            List<LoggingEntry> list = container.GetLoggingList();

            this.renderedList.Clear();
            this.renderedLineCountList.Clear();

            foreach (LoggingEntry logging in list)
            {
                if (!ShouldShow(logging))
                    continue;

                if (consoleVM.Collapse)
                {
                    renderedList.Add(logging.LoggingData);
                    renderedLineCountList.Add(logging.Count);
                }
                else
                    renderedList.AddRange(logging.LoggingDatas);

            }

            int count = renderedList.Count;
            var viewRect = new Rect(0, 0, areaRect.width - 20, count * lineHeight);

            if (viewRect.height >= areaRect.height && this.loggingVerticalScrollBarPercent > 0.95f)
                loggingPanelScrollPosition.y = viewRect.height - areaRect.height;

            loggingPanelScrollPosition = GUI.BeginScrollView(areaRect, loggingPanelScrollPosition, viewRect);
            if (viewRect.height >= areaRect.height)
                this.loggingVerticalScrollBarPercent = loggingPanelScrollPosition.y / (viewRect.height - areaRect.height);

            int firstIndex = (int)(loggingPanelScrollPosition.y / lineHeight);
            int lastIndex = firstIndex + (int)(areaRect.height / lineHeight);

            firstIndex = Mathf.Clamp(firstIndex - 5, 0, count);
            lastIndex = Mathf.Clamp(lastIndex + 5, 0, count);

            if (this.selectedIndex >= count)
                this.selectedIndex = -1;

            for (int i = firstIndex; i < lastIndex; i++)
            {
                LoggingData data = renderedList[i];
                var content = this.GetLogLineGUIContent(data);
                var lineStyle = (i % 2 == 0) ? entryStyleBackEven : entryStyleBackOdd;

                bool selected = i == selectedIndex;
                lineStyle.normal = selected ? GUI.skin.GetStyle(lineStyle.name).onNormal : GUI.skin.GetStyle(lineStyle.name).normal;

                if (GUI.Button(new Rect(0f, i * lineHeight, viewRect.width, lineHeight), content, lineStyle))
                {
                    if (selected)
                    {
                        if (EditorApplication.timeSinceStartup - lastClickTime < doubleClickInterval)
                        {
                            lastClickTime = 0;
                            OpenSourceFile(data.LocationInfo);
                        }
                        else
                        {
                            lastClickTime = EditorApplication.timeSinceStartup;
                        }
                    }
                    else
                    {
                        this.selectedIndex = i;
                        lastClickTime = EditorApplication.timeSinceStartup;
                    }
                }

                if (consoleVM.Collapse)
                {
                    int logCount = renderedLineCountList[i];
                    GUIContent countContent = new GUIContent(logCount < 100 ? logCount.ToString() : "99+");
                    var size = countBadgeStyle.CalcSize(countContent);
                    size.x = Mathf.Clamp(size.x + 5, 20, 30);
                    size.y = Mathf.Clamp(size.y, 20, lineHeight);
                    GUI.Label(new Rect(viewRect.width - 15f - size.x / 2f, i * lineHeight + (lineHeight - size.y) / 2f, size.x, size.y), countContent, countBadgeStyle);
                }
            }

            GUI.EndScrollView();
        }

        void DrawLoggingDetail()
        {
            var areaRect = new Rect(0f, this.verticalSplitterLineRect.y + this.splitterRectHeight / 2f, position.width, this.position.height * (1f - this.verticalSplitterPercent) - splitterRectHeight / 2f);
            GUILayout.BeginArea(areaRect);
            detailPanelScrollPosition = EditorGUILayout.BeginScrollView(detailPanelScrollPosition);

            if (this.selectedIndex >= 0 && this.selectedIndex < this.renderedList.Count)
            {
                var data = renderedList[selectedIndex];
                EditorGUILayout.SelectableLabel(this.GetLogDetailContent(data), detailStyle, GUILayout.Width(areaRect.width - 10), GUILayout.Height(areaRect.height - 10));
            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        void DrawVerticalSplitter()
        {
            if (splitterLineTexture == null)
                return;

            verticalSplitterLineRect.Set(verticalSplitterLineRect.x, position.height * verticalSplitterPercent, position.width, 1);
            verticalSplitterRect.Set(verticalSplitterLineRect.x, verticalSplitterLineRect.y - splitterRectHeight / 2f, verticalSplitterLineRect.width, splitterRectHeight);

            GUI.DrawTexture(verticalSplitterLineRect, splitterLineTexture);
            EditorGUIUtility.AddCursorRect(verticalSplitterRect, MouseCursor.ResizeVertical);

            if (Event.current.type == EventType.MouseDown && verticalSplitterRect.Contains(Event.current.mousePosition))
                resizingVerticalSplitter = true;

            if (resizingVerticalSplitter)
            {
                verticalSplitterPercent = Mathf.Clamp(Event.current.mousePosition.y / this.position.height, 0.15f, 0.9f);
                verticalSplitterLineRect.y = position.height * verticalSplitterPercent;
            }

            if (Event.current.type == EventType.MouseUp)
                resizingVerticalSplitter = false;
        }

        private bool Button(GUIContentData model)
        {
            if (model == null)
                return false;

            return GUILayout.Button(model.Content, model.Style, model.LayoutOptions);
        }

        private bool Toggle(GUISwitchContentData model, Action onValueChanged = null)
        {
            if (model == null)
                return false;

            var result = GUILayout.Toggle(model.Value, model.Content, model.Style, model.LayoutOptions);
            if (result != model.Value)
            {
                model.Value = result;
                if (onValueChanged != null)
                    onValueChanged();
            }
            return result;
        }

        private bool OpenSourceFile(Log.Log4Net.LocationInfo location)
        {
            if (location == null || location.StackFrames == null)
                return false;

            foreach (var frame in location.StackFrames)
            {
                try
                {
                    if (frame == null)
                        continue;

                    string fileName = frame.FileName;
                    if (string.IsNullOrEmpty(fileName))
                        continue;

                    var dir = Directory.GetCurrentDirectory().ToString();
                    if (!fileName.StartsWith(dir))
                        continue;

                    if (!File.Exists(fileName))
                        continue;

                    if (Regex.IsMatch(fileName, @"Log4NetLogImpl.cs"))
                        continue;

                    if (InternalEditorUtility.OpenFileAtLineExternal(fileName, frame.LineNumber))
                        return true;
                }
                catch (Exception) { }
            }

            return false;
        }

        [Serializable]
        class GUIContentData
        {
            protected bool dirty;
            protected GUIContent content;
            protected GUIStyle style;
            protected GUILayoutOption[] layoutOptions;

            public GUIContentData(GUIContent content, GUIStyle style)
            {
                this.dirty = true;
                this.content = content;
                this.style = style;
                Vector2 size = style.CalcSize(this.content);
                this.layoutOptions = new GUILayoutOption[] { GUILayout.Width(size.x) };
            }

            public string Text
            {
                get { return this.content.text; }
                set
                {
                    this.content.text = value;
                    this.dirty = true;
                }
            }

            public Texture Image
            {
                get { return this.content.image; }
                set
                {
                    this.content.image = value;
                    this.dirty = true;
                }
            }

            public GUIContent Content
            {
                get { return this.content; }
            }

            public GUIStyle Style
            {
                get { return this.style; }
            }


            public GUILayoutOption[] LayoutOptions
            {
                get
                {
                    if (this.dirty)
                    {
                        Vector2 size = style.CalcSize(this.Content);
                        this.layoutOptions = new GUILayoutOption[] { GUILayout.Width(size.x) };
                        this.dirty = false;
                    }
                    return this.layoutOptions;
                }
            }
        }


        [Serializable]
        class GUISwitchContentData
        {
            protected bool dirty;
            protected bool value;
            protected GUIContent contentOn;
            protected GUIContent contentOff;
            protected GUIStyle style;
            protected GUILayoutOption[] layoutOptions;

            public GUISwitchContentData(bool value, GUIContent contentOn, GUIStyle style) : this(value, contentOn, null, style)
            {
            }

            public GUISwitchContentData(bool value, GUIContent contentOn, GUIContent contentOff, GUIStyle style)
            {
                this.dirty = true;
                this.value = value;
                this.contentOn = contentOn;
                this.contentOff = contentOff != null ? contentOff : contentOn;
                this.style = style;
                this.layoutOptions = null;
            }

            public bool Value
            {
                get { return this.value; }
                set
                {
                    if (this.value == value)
                        return;

                    this.value = value;
                    this.dirty = true;
                }
            }

            public string Text
            {
                get { return this.Content.text; }
                set
                {
                    if (string.Equals(this.Content.text, value))
                        return;

                    this.Content.text = value;
                    this.dirty = true;
                }
            }

            public Texture Image
            {
                get { return this.Content.image; }
                set
                {
                    this.Content.image = value;
                    this.dirty = true;
                }
            }

            public GUIContent Content
            {
                get { return this.value ? this.contentOn : this.contentOff; }
            }

            public GUIStyle Style
            {
                get { return this.style; }
            }

            public GUILayoutOption[] LayoutOptions
            {
                get
                {
                    if (this.dirty)
                    {
                        Vector2 size = style.CalcSize(this.Content);
                        this.layoutOptions = new GUILayoutOption[] { GUILayout.Width(size.x) };
                        this.dirty = false;
                    }
                    return this.layoutOptions;
                }
            }
        }

    }
}