using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class TableAnalyzer {

    public string sourceText { get; private set; }
    public FetchType type;
    public List<ClassInfomation> result;

    public TableAnalyzer(string sourceText, FetchType type) {
        this.sourceText = sourceText;
        this.type = type;
        ProcessText(sourceText);
    }

    public void ProcessText(string rawText) {
        List<ClassInfomation> dataList = new List<ClassInfomation>();

        string[] weeklyData = CropByWeek(rawText);
        for (int i = 1; i < weeklyData.Length; i++) {
            string weekData = weeklyData[i];
            //print("Week]" + weekData);
            string[] timeData = CropByTime(weekData);
            string[][] dailyDatas = { CropByDay(timeData[0]),
                                      CropByDay(timeData[1]),
                                      CropByDay(timeData[2]) };
            for (int j = 1; j < dailyDatas[0].Length; j++) {
                string date;
                string[] moContents = FetchContentAtMorning(dailyDatas[0][i], out date);

                string[] afContents = FetchContentOther(dailyDatas[1][i]);
                string[] evContents = FetchContentOther(dailyDatas[2][i]);

                ClassInfomation tempInfo = new ClassInfomation();
                if (TryConvertStringsToInfo(moContents, date, ClassSession.Morning, type, ref tempInfo)) {
                    dataList.Add(tempInfo);
                }
                if (TryConvertStringsToInfo(afContents, date, ClassSession.Afternoom, type, ref tempInfo)) {
                    dataList.Add(tempInfo);
                }
                if (TryConvertStringsToInfo(evContents, date, ClassSession.Evening, type, ref tempInfo)) {
                    dataList.Add(tempInfo);
                }
            }
        }

        result = dataList;
    }

    ClassInfomation ConvertStringsToInfo(string[] contents, string date, ClassSession session, FetchType type) {
        if (contents == null) {
            return new ClassInfomation();
        }
        switch (type) {
            case FetchType.Teacher:
                return new ClassInfomation() {
                    date = DateTime.Parse(date),
                    session = session,
                    classCode = contents[0],
                    className = contents[1],
                    classRoom = contents[2]
                };
            case FetchType.ClassCode:
                return new ClassInfomation() {
                    date = DateTime.Parse(date),
                    session = session,
                    teacher = contents[0],
                    className = contents[1],
                    classRoom = contents[2]
                };
            default:
                throw new Exception();
        }
    }

    bool TryConvertStringsToInfo(string[] contents, string date, ClassSession session, FetchType type, ref ClassInfomation info) {
        if (contents == null) {
            return false;
        }
        switch (type) {
            case FetchType.Teacher:
                info = new ClassInfomation() {
                    date = DateTime.Parse(date),
                    session = session,
                    classCode = contents[0],
                    className = contents[1],
                    classRoom = contents[2]
                };
                break;
            case FetchType.ClassCode:
                info = new ClassInfomation() {
                    date = DateTime.Parse(date),
                    session = session,
                    teacher = contents[0],
                    className = contents[1],
                    classRoom = contents[2]
                };
                break;
            default:
                return false;
        }

        return true;
    }

    string[] CropByWeek(string rawData) {
        List<string> ls = new List<string>(Regex.Split(rawData, @"<p[^第]*第<br>\d<br>週[^<]*<\/td>"));
        ls.RemoveAt(0);
        return ls.ToArray();
    }
    
    string[] CropByTime(string weekData) {
        return Regex.Split(weekData, @"<\/tr>[^<]*<tr>");
    }

    string[] CropByDay(string timeData) {
        var matchs = Regex.Matches(timeData, @"(?<=<td)(.*)(?=<\/td>)");
        string[] dayData = new string[matchs.Count];
        for (int i = 0; i < matchs.Count; i++) {
            dayData[i] = matchs[i].Value;
        }
        return dayData;
    }

    string[] FetchContentAtMorning(string dayData, out string date) {
        dayData = dayData.Replace(" ", "").Replace("　", "");
        var matchGrp = Regex.Match(dayData, @"<font[^>]*>([^<]*)<\/font><br>(.+$)").Groups;
        date = matchGrp[1].Value.Replace("／", "/");
        var contentMatch = Regex.Match(matchGrp[2].Value, @"<b>([\w]+)<\/b><br><u>([^<]+)<\/u><br><b>([\w]+)<\/b>");
        if (!contentMatch.Success) {
            return null;
        }
        var contentGrp = contentMatch.Groups;
        string[] content = new string[3];
        for (int i = 1; i < contentGrp.Count; i++) {
            content[i - 1] = contentGrp[i].Value;
        }

        return content;
    }

    string[] FetchContentOther(string dayData) {
        dayData = dayData.Replace(" ", "").Replace("　", "");
        var matchGrp = Regex.Match(dayData, "class=\".+\">(.+$)").Groups;
        var contentMatch = Regex.Match(matchGrp[1].Value, @"<b>([\w]+)<\/b><br><u>([^<]+)<\/u><br><b>([\w]+)<\/b>");
        if (!contentMatch.Success) {
            return null;
        }
        var contentGrp = contentMatch.Groups;
        string[] content = new string[3];
        for (int i = 1; i < contentGrp.Count; i++) {
            content[i - 1] = contentGrp[i].Value;
        }

        return content;
    }
}

[System.Serializable]
public struct ClassInfomation {
    public DateTime date;
    public ClassSession session;
    public string teacher;
    public string classCode;
    public string classRoom;
    public string className;

    public override string ToString() {
        return $"日期:{date},時間:{session},老師:{teacher},班級:{classCode},教室:{classRoom},課程:{className}";
    }
}

public enum ClassSession {
    Morning,
    Afternoom,
    Evening
}