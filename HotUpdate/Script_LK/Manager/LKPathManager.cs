using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LKPathManager {
    private static Dictionary<int, LKPathInfo> dic_path = new Dictionary<int, LKPathInfo>();
    public static LKPathInfo PreLoad(int id) {//预加载
        LKPathInfo path;
        if (dic_path.TryGetValue(id, out path) == false) {
            path = new LKPathInfo(false, true);
            path.ID = id;
            string[] lines = ResManager.LoadLines(GameEnum.Fish_LK, LKGameConfig.Path_Path + id);
            string[] pos_str;
            float x, y, z;
            foreach (var line in lines) {
                if (string.IsNullOrEmpty(line)) {
                    continue;
                }
                pos_str = line.TrimStart('(').TrimEnd(')').Split(',');
                if (float.TryParse(pos_str[0], out x) == false) {
                    x = 0;
                }
                if (float.TryParse(pos_str[1], out y) == false) {
                    y = 0;
                }
                //if (float.TryParse(pos_str[2], out z) == false) {//z轴没有用到
                //    z = 0;
                //}
                path.AddPos(LKPathInfo.NodeType.Frame, new Vector2(x, y));
            }
            dic_path.Add(id, path);
        }
        return path;
    }
    public static LKPathInfo GetPath(int id) {//根据路径ID获取路径点
        return LKPathManager.PreLoad(id);
    }
    public static void Clear() {
        dic_path.Clear();
    }

    public static List<LKPathInfo> GetCirclePath(int count) {//圆形出鱼路径
        float screen_width = LKGameConfig.ScreenWidth;
        //float screen_height = 768;
        //int kResolutionWidth = 1366;
        //int kResolutionHeight = 768;

        List<LKPathInfo> path_list = new List<LKPathInfo>();
        float angle;
        LKPathInfo info;
        for (int i = 0; i < count; ++i) {
            angle = 360 * i / count;
            info = new LKPathInfo(true);
            //info.DelayTime = 0;
            info.AddPos(LKPathInfo.NodeType.Linear, Vector2.zero, angle);
            info.AddPos(LKPathInfo.NodeType.Linear, Tools.Rotate(new Vector2(screen_width, 0), angle), angle);
            path_list.Add(info);
        }
        return path_list;
    }

    //蝴蝶鱼阵
    public static List<LKPathInfo> BuildSceneKind1Trace() {
        float screen_width = LKGameConfig.ScreenWidth;
        float screen_height = LKGameConfig.ScreenHeight;

        List<Vector2> fish_pos;
        List<LKPathInfo> path_list = new List<LKPathInfo>();
        LKPathInfo info;

        float kVScale = 1.0f;
        float kRadius = (screen_height - (240 * kVScale)) / 2;
        float kSpeed = 1.5f;
        Vector2 center = new Vector2(screen_width + kRadius, kRadius + 120 * kVScale);

        fish_pos = Tools.BuildCircle(center, kRadius, 100);
        for (int i = 0; i < 100; ++i) {
            info = new LKPathInfo(false, kSpeed);
            info.AddPos(LKPathInfo.NodeType.Linear,fish_pos[i]);
            info.AddPos(LKPathInfo.NodeType.Linear, new Vector2(-2 * kRadius, fish_pos[i].y));
            path_list.Add(info);
        }
        float kRotateRadian1 = 45;
        float kRotateRadian2 = 135;
        float kRadiusSmall = kRadius / 2;
        float kRadiusSmall1 = kRadius / 3;
        Vector2[] center_small = new Vector2[4];
        center_small[0] = center + Tools.Rotate(new Vector2(kRadiusSmall, 0), -kRotateRadian2);
        center_small[1] = center + Tools.Rotate(new Vector2(kRadiusSmall, 0), -kRotateRadian1);
        center_small[2] = center + Tools.Rotate(new Vector2(kRadiusSmall, 0), kRotateRadian2);
        center_small[3] = center + Tools.Rotate(new Vector2(kRadiusSmall, 0), kRotateRadian1);

        fish_pos = Tools.BuildCircle(center_small[0], kRadiusSmall1, 17);
        for (int i = 0; i < 17; ++i) {
            info = new LKPathInfo(false, kSpeed);
            info.AddPos(LKPathInfo.NodeType.Linear, fish_pos[i]);
            info.AddPos(LKPathInfo.NodeType.Linear, new Vector2(-2 * kRadius, fish_pos[i].y));
            path_list.Add(info);
        }
        fish_pos = Tools.BuildCircle(center_small[1], kRadiusSmall1, 17);
        for (int i = 0; i < 17; ++i) {
            info = new LKPathInfo(false, kSpeed);
            info.AddPos(LKPathInfo.NodeType.Linear, fish_pos[i]);
            info.AddPos(LKPathInfo.NodeType.Linear, new Vector2(-2 * kRadius, fish_pos[i].y));
            path_list.Add(info);
        }
        fish_pos = Tools.BuildCircle(center_small[2], kRadiusSmall1, 30);
        for (int i = 0; i < 30; ++i) {
            info = new LKPathInfo(false, kSpeed);
            info.AddPos(LKPathInfo.NodeType.Linear, fish_pos[i]);
            info.AddPos(LKPathInfo.NodeType.Linear, new Vector2(-2 * kRadius, fish_pos[i].y));
            path_list.Add(info);
        }
        fish_pos = Tools.BuildCircle(center_small[3], kRadiusSmall1, 30);
        for (int i = 0; i < 30; ++i) {
            info = new LKPathInfo(false, kSpeed);
            info.AddPos(LKPathInfo.NodeType.Linear, fish_pos[i]);
            info.AddPos(LKPathInfo.NodeType.Linear, new Vector2(-2 * kRadius, fish_pos[i].y));
            path_list.Add(info);
        }
        fish_pos = Tools.BuildCircle(center, kRadiusSmall / 2, 15);
        for (int i = 0; i < 15; ++i) {
            info = new LKPathInfo(false, kSpeed);
            info.AddPos(LKPathInfo.NodeType.Linear, fish_pos[i]);
            info.AddPos(LKPathInfo.NodeType.Linear, new Vector2(-2 * kRadius, fish_pos[i].y));
            path_list.Add(info);
        }
        info = new LKPathInfo(true, kSpeed);
        info.AddPos(LKPathInfo.NodeType.Linear, new Vector3(center.x, center.y, 180));
        info.AddPos(LKPathInfo.NodeType.Linear, new Vector3(-2 * kRadius, center.y, 180));
        path_list.Add(info);
        return path_list;
    }
    //上下等待 中间左右行走  中间走完上下继续走
    public static List<LKPathInfo> BuildSceneKind2Trace() {
        float screen_width = LKGameConfig.ScreenWidth;
        float screen_height = LKGameConfig.ScreenHeight;

        List<LKPathInfo> path_list = new List<LKPathInfo>();
        LKPathInfo info;

        float kHScale = 1;
        float kVScale = 1;
        float kStopExcursion = 150f * kVScale;
        float kHExcursion = 27f * kHScale / 2;
        float kSmallFishInterval = (screen_width - kHExcursion * 2) / 100;
        float kSmallFishHeight = 65f * kVScale;
        float kSpeed = 3f * kHScale;
        
        float[] big_fish_width = { 270 * kHScale, 226 * kHScale, 375 * kHScale, 420 * kHScale, 540 * kHScale, 454 * kHScale, 600 * kHScale };
        float[] big_fish_excursion = new float[7];
        for (int i = 0; i < 7; ++i) {
            big_fish_excursion[i] = big_fish_width[i];
            for (int j = 0; j < i; ++j) {
                big_fish_excursion[i] += big_fish_width[j];
            }
        }
        int small_height = (int)kSmallFishHeight * 3;
        float x, y;
        float wait_dis = screen_width + 2*big_fish_width[6]-kSmallFishHeight;
        for (int i = 0; i < 100; ++i) {
            info = new LKPathInfo(false, kSpeed);
            x = kHExcursion + i * kSmallFishInterval;
            info.AddPos(LKPathInfo.NodeType.Linear, new Vector2(x, -kSmallFishHeight - Random.Range(0, small_height)));
            info.AddPos(LKPathInfo.NodeType.Linear, new Vector2(x, kStopExcursion));
            info.AddWait(wait_dis);
            info.AddPos(LKPathInfo.NodeType.Linear, new Vector2(x, screen_height + kSmallFishHeight));
            path_list.Add(info);
        }
        for (int i = 0; i < 100; ++i) {
            info = new LKPathInfo(false, kSpeed);
            x = kHExcursion + i * kSmallFishInterval;
            info.AddPos(LKPathInfo.NodeType.Linear, new Vector2(x, screen_height + kSmallFishHeight + Random.Range(0, small_height)));
            info.AddPos(LKPathInfo.NodeType.Linear, new Vector2(x, screen_height - kStopExcursion));
            info.AddWait(wait_dis);
            info.AddPos(LKPathInfo.NodeType.Linear, new Vector2(x, -kSmallFishHeight));
            path_list.Add(info);
        }

        float y_excursoin = 250 * kVScale / 2;
        for (int i = 0; i < 7; ++i) {
            info = new LKPathInfo(false, kSpeed);
            y = screen_height / 2 - y_excursoin;
            info.AddPos(LKPathInfo.NodeType.Linear, new Vector2(-big_fish_excursion[i], y));
            info.AddPos(LKPathInfo.NodeType.Linear, new Vector2(screen_width + big_fish_width[i], y));
            path_list.Add(info);
        }
        for (int i = 0; i < 7; ++i) {
            info = new LKPathInfo(false, kSpeed);
            y = screen_height / 2 + y_excursoin;
            info.AddPos(LKPathInfo.NodeType.Linear, new Vector2(screen_width + big_fish_excursion[i], y));
            info.AddPos(LKPathInfo.NodeType.Linear, new Vector2(-big_fish_width[i], y));
            path_list.Add(info);
        }

        return path_list;
    }
    //左右圆环对冲
    public static List<LKPathInfo> BuildSceneKind3Trace() {
        float screen_width = LKGameConfig.ScreenWidth;
        float screen_height = LKGameConfig.ScreenHeight;

        List<Vector2> fish_pos;
        List<LKPathInfo> path_list = new List<LKPathInfo>();
        LKPathInfo info;

        float kRadius = (screen_height - 240) / 2;
        float kSpeed = 1.5f;
        Vector2 center = new Vector2(screen_width + kRadius, kRadius + 120);

        fish_pos = Tools.BuildCircle(center, kRadius, 50);
        for (int i = 0; i < 50; ++i) {
            info = new LKPathInfo(false, kSpeed);
            info.AddPos(LKPathInfo.NodeType.Linear, fish_pos[i]);
            info.AddPos(LKPathInfo.NodeType.Linear, new Vector2(-2 * kRadius, fish_pos[i].y));
            path_list.Add(info);
        }

        fish_pos = Tools.BuildCircle(center, kRadius * 40 / 50, 40);
        for (int i = 0; i < 40; ++i) {
            info = new LKPathInfo(false, kSpeed);
            info.AddPos(LKPathInfo.NodeType.Linear, fish_pos[i]);
            info.AddPos(LKPathInfo.NodeType.Linear, new Vector2(-2 * kRadius, fish_pos[i].y));
            path_list.Add(info);
        }

        fish_pos = Tools.BuildCircle(center, kRadius * 30 / 50, 30);
        for (int i = 0; i < 30; ++i) {
            info = new LKPathInfo(false, kSpeed);
            info.AddPos(LKPathInfo.NodeType.Linear,fish_pos[i]);
            info.AddPos(LKPathInfo.NodeType.Linear,new Vector2(-2 * kRadius, fish_pos[i].y));
            path_list.Add(info);
        }

        info = new LKPathInfo(false, kSpeed);
        info.AddPos(LKPathInfo.NodeType.Linear,center);
        info.AddPos(LKPathInfo.NodeType.Linear,new Vector2(-2 * kRadius, center.y));
        path_list.Add(info);

        center.x = -kRadius;
        fish_pos = Tools.BuildCircle(center, kRadius, 50);
        for (int i = 0; i < 50; ++i) {
            info = new LKPathInfo(false, kSpeed);
            info.AddPos(LKPathInfo.NodeType.Linear,fish_pos[i]);
            info.AddPos(LKPathInfo.NodeType.Linear,new Vector2(screen_width + 2 * kRadius, fish_pos[i].y));
            path_list.Add(info);
        }

        fish_pos = Tools.BuildCircle(center, kRadius * 40 / 50, 40);
        for (int i = 0; i < 40; ++i) {
            info = new LKPathInfo(false, kSpeed);
            info.AddPos(LKPathInfo.NodeType.Linear,fish_pos[i]);
            info.AddPos(LKPathInfo.NodeType.Linear,new Vector2(screen_width + 2 * kRadius, fish_pos[i].y));
            path_list.Add(info);
        }

        fish_pos = Tools.BuildCircle(center, kRadius * 30 / 50, 30);
        for (int i = 0; i < 30; ++i) {
            info = new LKPathInfo(false, kSpeed);
            info.AddPos(LKPathInfo.NodeType.Linear,fish_pos[i]);
            info.AddPos(LKPathInfo.NodeType.Linear,new Vector2(screen_width + 2 * kRadius, fish_pos[i].y));
            path_list.Add(info);
        }

        info = new LKPathInfo(false, kSpeed);
        info.AddPos(LKPathInfo.NodeType.Linear,center);
        info.AddPos(LKPathInfo.NodeType.Linear,new Vector2(screen_width + 2 * kRadius, center.y));
        path_list.Add(info);

        return path_list;
    }
    //四角对冲
    public static List<LKPathInfo> BuildSceneKind4Trace() {
        float screen_width = LKGameConfig.ScreenWidth;
        float screen_height = LKGameConfig.ScreenHeight;

        List<LKPathInfo> path_list = new List<LKPathInfo>();
        LKPathInfo info;

        float kSpeed = 3f;
        float kFishWidth = 512;
        float kFishHeight = 304;


        // 左下
        Vector2 start_pos = new Vector2(0f, screen_height - kFishHeight / 2);
        Vector2 target_pos = new Vector2(screen_width - kFishHeight / 2, 0f);
        float angle_rad = Mathf.Acos((target_pos.x - start_pos.x) / Vector2.Distance(target_pos, start_pos));
        float radius = kFishWidth * 4;
        float length = radius + kFishWidth / 2f;
        Vector2 center_pos = new Vector2(-length * Mathf.Cos(angle_rad), start_pos.y + length * Mathf.Sin(angle_rad));
        Vector2 pos;
        Vector2 pos2;
        pos2.x = target_pos.x + kFishWidth;
        pos2.y = target_pos.y - kFishHeight;
        for (int i = 0; i < 8; ++i) {
            if (radius < 0f) {
                pos.x = center_pos.x + radius * Mathf.Cos(angle_rad);
                pos.y = center_pos.y - radius * Mathf.Sin(angle_rad);
            } else {
                pos.x = center_pos.x - radius * Mathf.Cos(angle_rad + Mathf.PI);
                pos.y = center_pos.y + radius * Mathf.Sin(angle_rad + Mathf.PI);
            }
            info = new LKPathInfo(false, kSpeed);
            info.AddPos(LKPathInfo.NodeType.Linear,pos);
            info.AddPos(LKPathInfo.NodeType.Linear,pos2);
            path_list.Add(info);

            radius -= kFishWidth;
        }

        start_pos.x = kFishHeight / 2;
        start_pos.y = screen_height;
        target_pos.x = screen_width;
        target_pos.y = kFishHeight / 2;
        angle_rad = Mathf.Acos((target_pos.x - start_pos.x) / Vector2.Distance(target_pos, start_pos));
        radius = kFishWidth * 4;
        length = radius + kFishWidth / 2f;
        center_pos.x = start_pos.x - length * Mathf.Cos(angle_rad);
        center_pos.y = start_pos.y + length * Mathf.Sin(angle_rad);
        pos2.x = target_pos.x + kFishWidth;
        pos2.y = target_pos.y - kFishHeight;
        for (int i = 0; i < 8; ++i) {
            if (radius < 0f) {
                pos.x = center_pos.x + radius * Mathf.Cos(angle_rad);
                pos.y = center_pos.y - radius * Mathf.Sin(angle_rad);
            } else {
                pos.x = center_pos.x - radius * Mathf.Cos(angle_rad + Mathf.PI);
                pos.y = center_pos.y + radius * Mathf.Sin(angle_rad + Mathf.PI);
            }
            info = new LKPathInfo(false, kSpeed);
            info.AddPos(LKPathInfo.NodeType.Linear,pos);
            info.AddPos(LKPathInfo.NodeType.Linear,pos2);
            path_list.Add(info);

            radius -= kFishWidth;
        }

        // 右下
        start_pos.x = screen_width - kFishHeight / 2;
        start_pos.y = screen_height;
        target_pos.x = 0f;
        target_pos.y = kFishHeight / 2;
        angle_rad = Mathf.Acos((start_pos.x - target_pos.x) / Vector2.Distance(target_pos, start_pos));
        radius = kFishWidth * 4;
        length = radius + kFishWidth / 2f;
        center_pos.x = start_pos.x + length * Mathf.Cos(angle_rad);
        center_pos.y = start_pos.y + length * Mathf.Sin(angle_rad);
        pos2.x = target_pos.x - kFishWidth;
        pos2.y = target_pos.y - kFishHeight;
        for (int i = 0; i < 8; ++i) {
            if (radius < 0f) {
                pos.x = center_pos.x - radius * Mathf.Cos(angle_rad + Mathf.PI);
                pos.y = center_pos.y - radius * Mathf.Sin(angle_rad + Mathf.PI);
            } else {
                pos.x = center_pos.x - radius * Mathf.Cos(angle_rad);
                pos.y = center_pos.y - radius * Mathf.Sin(angle_rad);
            }
            info = new LKPathInfo(false, kSpeed);
            info.AddPos(LKPathInfo.NodeType.Linear,pos);
            info.AddPos(LKPathInfo.NodeType.Linear,pos2);
            path_list.Add(info);
            radius -= kFishWidth;
        }

        start_pos.x = screen_width;
        start_pos.y = screen_height - kFishHeight / 2;
        target_pos.x = kFishHeight / 2;
        target_pos.y = 0f;
        angle_rad = Mathf.Acos((start_pos.x - target_pos.x) / Vector2.Distance(target_pos, start_pos));
        radius = kFishWidth * 4;
        length = radius + kFishWidth / 2f;
        center_pos.x = start_pos.x + length * Mathf.Cos(angle_rad);
        center_pos.y = start_pos.y + length * Mathf.Sin(angle_rad);
        pos2.x = target_pos.x - kFishWidth;
        pos2.y = target_pos.y - kFishHeight;
        for (int i = 0; i < 8; ++i) {
            if (radius < 0f) {
                pos.x = center_pos.x - radius * Mathf.Cos(angle_rad + Mathf.PI);
                pos.y = center_pos.y - radius * Mathf.Sin(angle_rad + Mathf.PI);
            } else {
                pos.x = center_pos.x - radius * Mathf.Cos(angle_rad);
                pos.y = center_pos.y - radius * Mathf.Sin(angle_rad);
            }
            info = new LKPathInfo(false, kSpeed);
            info.AddPos(LKPathInfo.NodeType.Linear,pos);
            info.AddPos(LKPathInfo.NodeType.Linear,pos2);
            path_list.Add(info);
            radius -= kFishWidth;
        }

        // 右上
        start_pos.x = screen_width;
        start_pos.y = kFishHeight / 2;
        target_pos.x = kFishHeight / 2;
        target_pos.y = screen_height;
        angle_rad = Mathf.Acos((start_pos.x - target_pos.x) / Vector2.Distance(target_pos, start_pos));
        radius = kFishWidth * 4;
        length = radius + kFishWidth / 2f;
        center_pos.x = start_pos.x + length * Mathf.Cos(angle_rad);
        center_pos.y = start_pos.y - length * Mathf.Sin(angle_rad);
        pos2.x = target_pos.x - kFishWidth;
        pos2.y = target_pos.y + kFishHeight;
        for (int i = 0; i < 8; ++i) {
            if (radius < 0f) {
                pos.x = center_pos.x - radius * Mathf.Cos(-angle_rad - Mathf.PI);
                pos.y = center_pos.y - radius * Mathf.Sin(-angle_rad - Mathf.PI);
            } else {
                pos.x = center_pos.x - radius * Mathf.Cos(-angle_rad);
                pos.y = center_pos.y - radius * Mathf.Sin(-angle_rad);
            }
            info = new LKPathInfo(false, kSpeed);
            info.AddPos(LKPathInfo.NodeType.Linear,pos);
            info.AddPos(LKPathInfo.NodeType.Linear,pos2);
            path_list.Add(info);
            radius -= kFishWidth;
        }

        start_pos.x = screen_width - kFishHeight / 2;
        start_pos.y = 0f;
        target_pos.x = 0f;
        target_pos.y = screen_height - kFishHeight / 2;
        angle_rad = Mathf.Acos((start_pos.x - target_pos.x) / Vector2.Distance(target_pos, start_pos));
        radius = kFishWidth * 4;
        length = radius + kFishWidth / 2f;
        center_pos.x = start_pos.x + length * Mathf.Cos(angle_rad);
        center_pos.y = start_pos.y - length * Mathf.Sin(angle_rad);
        pos2.x = target_pos.x - kFishWidth;
        pos2.y = target_pos.y + kFishHeight;
        for (int i = 0; i < 8; ++i) {
            if (radius < 0f) {
                pos.x = center_pos.x - radius * Mathf.Cos(-angle_rad - Mathf.PI);
                pos.y = center_pos.y - radius * Mathf.Sin(-angle_rad - Mathf.PI);
            } else {
                pos.x = center_pos.x - radius * Mathf.Cos(-angle_rad);
                pos.y = center_pos.y - radius * Mathf.Sin(-angle_rad);
            }
            info = new LKPathInfo(false, kSpeed);
            info.AddPos(LKPathInfo.NodeType.Linear,pos);
            info.AddPos(LKPathInfo.NodeType.Linear,pos2);
            path_list.Add(info);
            radius -= kFishWidth;
        }

        // 左上
        start_pos.x = kFishHeight / 2;
        start_pos.y = 0f;
        target_pos.x = screen_width;
        target_pos.y = screen_height - kFishHeight / 2;
        angle_rad = Mathf.Acos((target_pos.x - start_pos.x) / Vector2.Distance(target_pos, start_pos));
        radius = kFishWidth * 4;
        length = radius + kFishWidth / 2f;
        center_pos.x = start_pos.x - length * Mathf.Cos(angle_rad);
        center_pos.y = start_pos.y - length * Mathf.Sin(angle_rad);
        pos2.x = target_pos.x + kFishWidth;
        pos2.y = target_pos.y + kFishHeight;
        for (int i = 0; i < 8; ++i) {
            if (radius < 0f) {
                pos.x = center_pos.x + radius * Mathf.Cos(angle_rad + Mathf.PI);
                pos.y = center_pos.y + radius * Mathf.Sin(angle_rad + Mathf.PI);
            } else {
                pos.x = center_pos.x + radius * Mathf.Cos(angle_rad);
                pos.y = center_pos.y + radius * Mathf.Sin(angle_rad);
            }
            info = new LKPathInfo(false, kSpeed);
            info.AddPos(LKPathInfo.NodeType.Linear,pos);
            info.AddPos(LKPathInfo.NodeType.Linear,pos2);
            path_list.Add(info);
            radius -= kFishWidth;
        }

        start_pos.x = 0f;
        start_pos.y = kFishHeight / 2;
        target_pos.x = screen_width - kFishHeight / 2;
        target_pos.y = screen_height;
        angle_rad = Mathf.Acos((target_pos.x - start_pos.x) / Vector2.Distance(target_pos, start_pos));
        radius = kFishWidth * 4;
        length = radius + kFishWidth / 2f;
        center_pos.x = start_pos.x - length * Mathf.Cos(angle_rad);
        center_pos.y = start_pos.y - length * Mathf.Sin(angle_rad);
        pos2.x = target_pos.x + kFishWidth;
        pos2.y = target_pos.y + kFishHeight;
        for (int i = 0; i < 8; ++i) {
            if (radius < 0f) {
                pos.x = center_pos.x + radius * Mathf.Cos(angle_rad + Mathf.PI);
                pos.y = center_pos.y + radius * Mathf.Sin(angle_rad + Mathf.PI);
            } else {
                pos.x = center_pos.x + radius * Mathf.Cos(angle_rad);
                pos.y = center_pos.y + radius * Mathf.Sin(angle_rad);
            }
            info = new LKPathInfo(false, kSpeed);
            info.AddPos(LKPathInfo.NodeType.Linear,pos);
            info.AddPos(LKPathInfo.NodeType.Linear,pos2);
            path_list.Add(info);
            radius -= kFishWidth;
        }
        return path_list;
    }

    private static  float angle_range(float angle) {
      while (angle <= -360) {
        angle += 360;
      }
      if (angle < 0f) angle += 360;
      while (angle >= 360) {
        angle -= 360;
      }
      return angle;
    }

    private static Vector2 GetTargetPoint(float screen_width, float screen_height, float src_x_pos, float src_y_pos, float angle) {
        //300 160
        angle = angle_range(angle);

        if (angle > 0f && angle < 90) {
            return new Vector2(screen_width + 300, src_y_pos + (screen_width - src_x_pos + 300) * Mathf.Tan(angle * Mathf.Deg2Rad));
        } else if (angle >= 90 && angle < 180) {
            return new Vector2(-300, src_y_pos - (src_x_pos + 300) * Mathf.Tan(angle * Mathf.Deg2Rad));
        } else if (angle >= 180 && angle < 270) {
            return new Vector2(-300, src_y_pos - (src_x_pos + 300) * Mathf.Tan(angle * Mathf.Deg2Rad));
        } else {
            return new Vector2(screen_width + 300, src_y_pos + (screen_width - src_x_pos + 300) * Mathf.Tan(angle * Mathf.Deg2Rad));
        }
    }
    //双环自传鱼阵
    public static List<LKPathInfo> BuildSceneKind5Trace() {
        float screen_width = LKGameConfig.ScreenWidth;
        float screen_height = LKGameConfig.ScreenHeight;

        List<LKPathInfo.PathInfo> fish_pos;
        List<LKPathInfo> path_list = new List<LKPathInfo>();
        LKPathInfo info;

        int fish_count = 0;
        float kRadius = (screen_height - 200) / 2;
        float kRotateSpeed = 1.5f;
        float kSpeed = 5f;
        Vector2[] center = new Vector2[2];
        center[0].x = screen_width / 4f;
        center[0].y = kRadius + 100;
        center[1].x = screen_width - screen_width / 4f;
        center[1].y = kRadius + 100;

        float kLFish1Rotate = 720f;
        float kRFish2Rotate = (720f + 90f);
        float kRFish5Rotate = (720f + 180f);
        float kLFish3Rotate = (720f + 180f + 45f);
        float kLFish4Rotate = (720f + 180f + 90f);
        float kRFish6Rotate = (720f + 180f + 90f + 30f);
        float kRFish7Rotate = (720f + 180f + 90f + 60f);
        float kLFish6Rotate = (720f + 180f + 90f + 60f + 30f);
        float kLFish18Rotate = (720f + 180f + 90f + 60f + 60f);
        float kRFish17Rotate = (720f + 180f + 90f + 60f + 60f + 30f);
        for (float rotate = 0f; rotate <= kLFish1Rotate; rotate += kRotateSpeed) {
            fish_pos = Tools.BuildCircle(center[0], kRadius, 40, rotate);
            for (int j = 0; j < 40; ++j) {
                while (path_list.Count <= j) {
                    path_list.Add(new LKPathInfo(true, true));
                }
                path_list[j].AddNode(fish_pos[j]);
            }
        }
        fish_count += 40;
        for (float rotate = 0f; rotate <= kRFish2Rotate; rotate += kRotateSpeed) {
            fish_pos = Tools.BuildCircle(center[1], kRadius, 40, rotate);
            for (int j = 0; j < 40; ++j) {
                while (path_list.Count <= fish_count + j) {
                    path_list.Add(new LKPathInfo(true, true));
                }
                path_list[fish_count + j].AddNode(fish_pos[j]);
            }
        }
        fish_count += 40;

        for (float rotate = 0f; rotate <= kRFish5Rotate; rotate += kRotateSpeed) {
            fish_pos = Tools.BuildCircle(center[1], kRadius - 34.5f, 40, rotate);
            for (int j = 0; j < 40; ++j) {
                while (path_list.Count <= fish_count + j) {
                    path_list.Add(new LKPathInfo(true, true));
                }
                path_list[fish_count + j].AddNode(fish_pos[j]);
            }
        }
        fish_count += 40;
        for (float rotate = 0f; rotate <= kLFish3Rotate; rotate += kRotateSpeed) {
            fish_pos = Tools.BuildCircle(center[0], kRadius - 36f, 40, rotate);
            for (int j = 0; j < 40; ++j) {
                while (path_list.Count <= fish_count + j) {
                    path_list.Add(new LKPathInfo(true, true));
                }
                path_list[fish_count + j].AddNode(fish_pos[j]);
            }
        }
        fish_count += 40;

        for (float rotate = 0f; rotate <= kLFish4Rotate; rotate += kRotateSpeed) {
            fish_pos = Tools.BuildCircle(center[0], kRadius - 36f - 56f, 24, rotate);
            for (int j = 0; j < 24; ++j) {
                while (path_list.Count <= fish_count + j) {
                    path_list.Add(new LKPathInfo(true, true));
                }
                path_list[fish_count + j].AddNode(fish_pos[j]);
            }
        }
        fish_count += 24;
        for (float rotate = 0f; rotate <= kRFish6Rotate; rotate += kRotateSpeed) {
            fish_pos = Tools.BuildCircle(center[1], kRadius - 34.5f - 58.5f, 24, rotate);
            for (int j = 0; j < 24; ++j) {
                while (path_list.Count <= fish_count + j) {
                    path_list.Add(new LKPathInfo(true, true));
                }
                path_list[fish_count + j].AddNode(fish_pos[j]);
            }
        }
        fish_count += 24;

        for (float rotate = 0f; rotate <= kRFish7Rotate; rotate += kRotateSpeed) {
            fish_pos = Tools.BuildCircle(center[1], kRadius - 34.5f - 58.5f - 65f, 13, rotate);
            for (int j = 0; j < 13; ++j) {
                while (path_list.Count <= fish_count + j) {
                    path_list.Add(new LKPathInfo(true, true));
                }
                path_list[fish_count + j].AddNode(fish_pos[j]);
            }
        }
        fish_count += 13;
        for (float rotate = 0f; rotate <= kLFish6Rotate; rotate += kRotateSpeed) {
            fish_pos = Tools.BuildCircle(center[0], kRadius - 36f - 56f - 68f, 13, rotate);
            for (int j = 0; j < 13; ++j) {
                while (path_list.Count <= fish_count + j) {
                    path_list.Add(new LKPathInfo(true, true));
                }
                path_list[fish_count + j].AddNode(fish_pos[j]);
            }
        }
        fish_count += 13;

        info = new LKPathInfo(true, true);
        for (float rotate = 0f; rotate <= kLFish18Rotate; rotate += kRotateSpeed) {
            info.AddPos(LKPathInfo.NodeType.Frame, center[0], -90 + rotate);
        }
        path_list.Add(info);
        fish_count += 1;

        info = new LKPathInfo(true, true);
        for (float rotate = 0f; rotate <= kRFish17Rotate; rotate += kRotateSpeed) {
            info.AddPos(LKPathInfo.NodeType.Frame, center[1], -90 + rotate);
        }
        path_list.Add(info);
        fish_count += 1;

        Vector2 pos2;
        LKPathInfo.PathInfo pos;
        for (int i = 0; i < path_list.Count; ++i) {
            pos = path_list[i].GetLastNode();
            pos2 = GetTargetPoint(screen_width, screen_height, pos.TargetPos.x, pos.TargetPos.y, pos.Angle);

            path_list[i].MoveSpd = kSpeed;
            path_list[i].AddPos(LKPathInfo.NodeType.Linear, pos2, -Tools.Angle(Vector2.right, pos2 - pos.TargetPos));
        }
        return path_list;
    }
    
    //上下对冲鱼阵
    public static List<LKPathInfo> BuildSceneKind6Trace() {
        float screen_width = LKGameConfig.ScreenWidth;
        float screen_height = LKGameConfig.ScreenHeight;

        List<LKPathInfo> path_list = new List<LKPathInfo>();
        LKPathInfo info;

        float kSpeed = Mathf.PI * 0.5f;

        Vector2 pos1;
        Vector2 pos2;
        for (int c = 0; c < 4; c++) {
            for (int i = 0; i < 30; i++) {
                pos1.x = screen_width * (i + 1) / 31;
                pos1.y = screen_height + 30;

                pos2.x = pos1.x;
                pos2.y = -screen_height * 40f;
                //由上往下
                info = new LKPathInfo(true, kSpeed);
                info.DelayTime = 6 * c;
                info.AddPos(LKPathInfo.NodeType.Linear, pos1, -90);
                info.AddPos(LKPathInfo.NodeType.Linear, pos2, -90);
                path_list.Add(info);
            }
            for (int i = 0; i < 30; i++) {
                pos1.x = screen_width * (i + 1) / 31;
                pos1.y = screen_height + 30;

                pos2.x = pos1.x;
                pos2.y = -screen_height * 40f;
                //由下网上
                info = new LKPathInfo(true, kSpeed);
                info.DelayTime = 6 * c;
                info.AddPos(LKPathInfo.NodeType.Linear, pos2, 90);
                info.AddPos(LKPathInfo.NodeType.Linear, pos1, 90);
                path_list.Add(info);
            }
        }

        List<LKPathInfo.PathInfo> path_1 = Tools.BuildSine(-200, screen_height / 2, screen_width + 200, screen_height / 2, 3f, Mathf.PI * 0.5f, 300, screen_width + 2 / 1.5f);
        List<LKPathInfo.PathInfo> path_2 = Tools.BuildSine(-200, screen_height / 2, screen_width + 200, screen_height / 2, 3f, -Mathf.PI * 0.5f, 300, screen_width + 2 / 1.5f);

        for (int i = 0; i < 9; ++i) {
            info = new LKPathInfo(false, true);
            if (i < 6) {
                info.DelayTime = i * 3f;
            } else if (i < 8) {
                info.DelayTime = i * 3.2f;
            } else {
                info.DelayTime = i * 3.4f;
            }
            for (int j = 0; j < path_2.Count; j++) {
                info.AddNode(path_2[j]);
            }
            path_list.Add(info);

            info = new LKPathInfo(false, true);
            if (i < 6) {
                info.DelayTime = i * 3f;
            } else if (i < 8) {
                info.DelayTime = i * 3.2f;
            } else {
                info.DelayTime = i * 3.4f;
            }
            for (int j = 0; j < path_1.Count; j++) {
                info.AddNode(path_1[j]);
            }
            path_list.Add(info);
        }
        return path_list;
    }

    //各种扇形出鱼
    public static List<LKPathInfo> BuildSceneKind7Trace() {
        float screen_width = LKGameConfig.ScreenWidth;
        float screen_height = LKGameConfig.ScreenHeight;

        List<LKPathInfo> path_list = new List<LKPathInfo>();
        LKPathInfo info;

        float kSpeed = 2;

        float center_x = screen_width / 2;
        float center_y = screen_height / 2;

        Vector2 pt;
        float angle;
        for (int c = 0; c < 9; c++) {
            for (int i = 0; i < 30; ++i) {
                pt.x = center_x;
                pt.y = center_y;
                angle = 360 * i / 30;
                info = new LKPathInfo(true, kSpeed);
                if (c < 6) {
                    info.DelayTime = c * 2;
                } else {
                    info.DelayTime = c * 2 + 2;
                }
                info.AddPos(LKPathInfo.NodeType.Linear, pt, angle);
                info.AddPos(LKPathInfo.NodeType.Linear, pt + Tools.Rotate(new Vector2(screen_width, 0), angle), angle);
                path_list.Add(info);
            }
        }

        Vector2[] pos_arr = {
            new Vector2(0 - center_x, 0 - center_y),
            new Vector2(screen_width - center_x, 0 - center_y),
            new Vector2(screen_width - center_x, screen_height - center_y),
            new Vector2(0 - center_x, screen_height - center_y),
        };

        for (int i = 0; i < 4; ++i) {
            pt.x = center_x;
            pt.y = center_y;
            angle = Tools.Angle(Vector2.right, pos_arr[i]);
            info = new LKPathInfo(true, kSpeed);
            info.DelayTime = 12;
            info.AddPos(LKPathInfo.NodeType.Linear, pt, angle);
            info.AddPos(LKPathInfo.NodeType.Linear, pt + Tools.Rotate(new Vector2(screen_width, 0), angle), angle);
            path_list.Add(info);
        }

        for (int i = 0; i < 2; ++i) {
            pt.x = center_x;
            pt.y = center_y;
            angle = i * 180;
            info = new LKPathInfo(true, kSpeed);
            info.DelayTime = 5;
            info.AddPos(LKPathInfo.NodeType.Linear, pt, angle);
            info.AddPos(LKPathInfo.NodeType.Linear, pt + Tools.Rotate(new Vector2(screen_width, 0), angle), angle);
            path_list.Add(info);
        }

        pt.x = center_x;
        pt.y = center_y;
        angle = Random.Range(0, 360);
        info = new LKPathInfo(true, kSpeed);
        info.DelayTime = 16;
        info.AddPos(LKPathInfo.NodeType.Linear, pt, angle);
        info.AddPos(LKPathInfo.NodeType.Linear, pt + Tools.Rotate(new Vector2(screen_width, 0), angle), angle);
        path_list.Add(info);

        return path_list;
    }

    //随机路径出鱼
    public static List<LKPathInfo> BuildSceneKind8Trace(uint tick_count, int count) {
        float kSpeed = 2;

        List<LKPathInfo> path_list = new List<LKPathInfo>();
        LKPathInfo info;
        for (int i = 0; i < count; i++) {
            int path_id = (int)(((tick_count / 10 * 10 + i) / 10 * 10) % LKGameConfig.PATH_MAX_COUNT);
            if (i < 100) {
                info = LKPathManager.GetPath(path_id).Clone();
                info.MoveSpd = kSpeed;
                info.DelayTime = (i % 60) * 0.5f;
                path_list.Add(info);
            } else if (i < 140) {
                info = LKPathManager.GetPath(path_id).Clone();
                info.MoveSpd = kSpeed;
                info.DelayTime = (i - 100) * 1.0f;
                path_list.Add(info);
            } else if (i < 180) {
                info = LKPathManager.GetPath(path_id).Clone();
                info.MoveSpd = kSpeed;
                info.DelayTime = (i - 140) * 1.0f;
                path_list.Add(info);
            } else {
                path_id = 3 + (i - 180);
                ushort[] stop_index = { 200, 100, 100 };
                info = LKPathManager.GetPath(path_id).Clone();
                info.MoveSpd = kSpeed;
                info.DelayTime = (i - 180) * 10.0f;
                if (info.IsFrame) {
                    info.InsertWait(stop_index[i - 180], 200);
                } else {
                    info.InsertWait(stop_index[i - 180] * kSpeed / LKGameConfig.FPS, 200 * kSpeed / LKGameConfig.FPS);
                }
                path_list.Add(info);
            }

        }
        return path_list;
    }
}
public class LKPathWait{
    public int StartFrame;//开始等待帧数
    public int WaitLen;//等待总时长
}
public class LKPathInfo {
    public int ID;
    public bool IsLockAngle;//是否锁定角度  z轴当作角度处理
    public float MoveSpd = -1;//移动速度  -1则忽略
    public bool IsFrame;//是否帧运行
    public float DelayTime;//延迟出现时间
    private List<PathInfo> PosList = new List<PathInfo>();
    private List<LKPathWait> WaitList = new List<LKPathWait>();

    public LKPathInfo()
        : this(false) {
    }
    public LKPathInfo(bool is_lock_angle)
        : this(is_lock_angle, false) {
    }
    public LKPathInfo(bool is_lock_angle, float spd) {
        this.IsLockAngle = is_lock_angle;
        this.MoveSpd = spd;
        this.IsFrame = false;
    }
    public LKPathInfo(bool is_lock_angle, bool is_frame) {
        this.IsLockAngle = is_lock_angle;
        this.MoveSpd = -1;
        this.IsFrame = is_frame;
    }
    public void AddNode(PathInfo info) {
        this.PosList.Add(info);
    }
    public void AddWait(float dis) {//等待距离
        PathInfo info = new PathInfo();
        info.NodeType = NodeType.Wait;
        info.Distance = dis;
        this.AddNode(info);
    }
    public void InsertWait(int start_frame, int len_frmae) {//插入时间长度
        bool is_add = true;
        for (int i = 0; i < this.WaitList.Count; i++) {
            if (this.WaitList[i].StartFrame > start_frame) {
                this.WaitList.Insert(0, new LKPathWait {
                    StartFrame = start_frame,
                    WaitLen = len_frmae,
                });
                is_add = false;
                break;
            }
        }
        if (is_add) {
            this.WaitList.Add(new LKPathWait {
                StartFrame = start_frame,
                WaitLen = len_frmae,
            });
        }
        //if (this.PosList.Count > start_frame) {
        //    PathInfo info = this.PosList[start_frame];
        //    this.PosList.Insert(start_frame, new PathInfo {
        //        Angle = info.Angle,
        //        Distance = len_frmae * 1f / LKGameConfig.FPS,
        //        NodeType = NodeType.Wait,
        //        TargetPos = info.TargetPos,
        //    });
        //}
    }
    public void InsertWait(float start, float dis) {//插入等待
        Vector2 pre_pos = Vector2.zero;
        PathInfo info;
        for (int i = 0; i < this.PosList.Count; i++) {
            if (this.PosList[i].NodeType == NodeType.Wait) {
                continue;
            } else {
                if (pre_pos == Vector2.zero) {
                    pre_pos = this.PosList[i].TargetPos;
                    continue;
                }
                info = this.PosList[i];
                float d = Vector2.Distance(pre_pos,info.TargetPos);
                if (start <= d) {
                    pre_pos = Vector2.Lerp(pre_pos,info.TargetPos,start/d);
                    this.PosList.Insert(i, new PathInfo {
                        Angle = info.Angle,
                        NodeType = info.NodeType,
                        TargetPos = pre_pos,
                    });
                    this.PosList.Insert(i + 1, new PathInfo {
                        Angle = info.Angle,
                        Distance = dis,
                        NodeType = NodeType.Wait,
                        TargetPos = pre_pos,
                    });
                    break;
                } else {
                    start -= d;
                }
            }
        }
    }
    public void AddPos(NodeType type, Vector2 pos) {
        this.AddNode(new PathInfo {
            TargetPos = pos,
            NodeType = type,
        });
    }
    public void AddPos(NodeType type, Vector2 pos, float angle) {
        this.AddNode(new PathInfo {
            TargetPos = pos,
            NodeType = type,
            Angle = angle,
        });
    }
    public PathInfo GetLastNode() {
        return this.PosList[this.PosList.Count - 1];
    }
    public LKPathInfo Clone() {
        LKPathInfo info = new LKPathInfo();
        info.IsLockAngle = this.IsLockAngle;
        info.MoveSpd = this.MoveSpd;
        info.IsFrame = this.IsFrame;
        info.DelayTime = this.DelayTime;
        info.PosList.AddRange(this.PosList);
        return info;
    }

    //逻辑计算
    public LKPathInfo(params Vector3[] path):this(false,false) {
        foreach (var item in path) {
            this.AddPos(NodeType.Linear, item);
        }
        this.InitData();
    }
    public enum NodeType {
        Linear = 1,//直线
        Bezier = 2,//贝塞尔曲线
        Frame = 3,//帧位置

        Wait = 101,//等待
    }
    public class PathInfo {
        public NodeType NodeType;
        public float Distance;
        public float Angle;
        public Vector2 TargetPos;
    }
    private bool is_init = false;
    public List<PathInfo> mPathList = new List<PathInfo>();
    public float TotalDistance;
    public void InitData() {//路径计算
        if (is_init == true) {
            return;
        }
        is_init = true;
        float dis;
        TotalDistance = 0;
        PathInfo info;
        Vector2 pre_pos = this.PosList[0].TargetPos;
        float angle = 0;
        foreach (var item in this.PosList) {
            if (item.NodeType == NodeType.Wait) {
                dis = item.Distance;
            } else {
                if (this.IsFrame) {
                    //if (item.NodeType != NodeType.Frame && this.MoveSpd > 0) {
                    //    dis = Vector2.Distance(pre_pos, item.TargetPos);
                    //    dis = dis / this.MoveSpd * LKGameConfig.PathSpd;
                    //} else {
                        dis = LKGameConfig.PathSpd;
                    //}
                } else {
                    dis = Vector2.Distance(pre_pos, item.TargetPos);
                }
                pre_pos = item.TargetPos;
                angle = -item.Angle;
            }
            this.TotalDistance += dis;

            info = new PathInfo();
            info.NodeType = item.NodeType;
            info.Distance = dis;
            info.Angle = angle;
            info.TargetPos = pre_pos;
            info.TargetPos.x = info.TargetPos.x - LKGameConfig.ScreenWidthHalf;
            info.TargetPos.y = LKGameConfig.ScreenHeightHalf - info.TargetPos.y;

            if (mPathList.Count == 0) {
                info.Distance = 0;
            }
            mPathList.Add(info);
        }
        if (this.IsFrame) {
            foreach (var item in this.WaitList) {
                this.TotalDistance += item.WaitLen * LKGameConfig.PathSpd;
            }
        }
    }
    public float GetAngleByDis(float dis) {//获取角度计算
        if (this.IsLockAngle == false) {//没有锁定状态直接返回0
            return 0;
        }
        for (int i = 1; i < this.mPathList.Count; i++) {
            if (dis < this.mPathList[i].Distance) {
                return this.mPathList[i].Angle;
            } else {
                dis -= this.mPathList[i].Distance;
            }
        }
        if (this.mPathList.Count > 0) {
            return this.mPathList[this.mPathList.Count - 1].Angle;
        } else {
            return 0;
        }
    }
    public Vector2 GetPosByDis(float dis) {
        if (this.mPathList.Count == 0) {
            return Vector2.zero;
        }
        if (this.IsFrame) {
            int frame = (int)(dis * LKGameConfig.FPS);
            foreach (var item in this.WaitList) {
                if (frame >= item.StartFrame) {
                    if (frame <= item.StartFrame + item.WaitLen) {
                        frame = item.StartFrame;
                        break;
                    } else {
                        frame -= item.WaitLen;
                    }
                } else {
                    break;
                }
            }
            if (this.mPathList.Count > frame) {
                return this.mPathList[frame].TargetPos;
            }
        } else {
            Vector2 pre_pos = this.mPathList[0].TargetPos;
            foreach (var item in this.mPathList) {
                if (dis < item.Distance) {
                    if (item.NodeType == NodeType.Wait) {
                        return pre_pos;
                    } else {
                        return Vector3.Lerp(pre_pos, item.TargetPos, dis / item.Distance);
                    }
                } else {
                    dis -= item.Distance;
                    if (item.NodeType != NodeType.Wait) {
                        pre_pos = item.TargetPos;
                    }
                }
            }
        }
        return this.mPathList[this.mPathList.Count - 1].TargetPos;
    }
    private static List<Vector2> __bezier = new List<Vector2>();
    public Vector2 GetPosByBezierDis(float dis) {//获取贝塞尔曲线
        if (this.mPathList.Count == 0) {
            return Vector2.zero;
        }
        if (this.TotalDistance == 0) {
            return this.mPathList[0].TargetPos;
        }
        __bezier.Clear();
        foreach (var item in this.mPathList) {
            __bezier.Add(item.TargetPos);
        }
        float t = dis / this.TotalDistance;
        for (int count = __bezier.Count - 1; count > 0; count--) {
            for (int i = 0; i < count; i++) {
                __bezier[i] = Vector2.Lerp(__bezier[i], __bezier[i + 1], t);
            }
        }
        return __bezier[0];
    }
}
