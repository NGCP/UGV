
using System;
using System.Collections.Generic;
using System.Text;

namespace ObstacleDetection
{
    public class SCIP_Writer
    {
        public static string requestSensorData(int start, int end, int grouping = 1, int skips = 0, int scans = 0)
        {
            return "MD" + start.ToString("D4") + end.ToString("D4") + grouping.ToString("D2") + skips.ToString("D1") + scans.ToString("D2") + "\n";
        }

        public static string setSCIP2Mode()
        {
            return "SCIP2.0" + "\n";
        }

        public static string turnOffLaser()
        {
            return "QT\n";
        }

        public static string resetSettings()
        {
            return "RS\n";
        }
    }

    public class SCIP_Reader
    {
        public static bool getSensorData(string get_command, ref long time_stamp, ref List<double> distances)
        {
            distances.Clear();
            string[] split_command = get_command.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (!split_command[0].StartsWith("MD"))
            {
                return false;
            }

            if (split_command[1].StartsWith("00"))
            {
                return true;
            }
            else if (split_command[1].StartsWith("99"))
            {
                time_stamp = SCIP_Reader.decode(split_command[2], 4);
                distance_data(split_command, 3, ref distances);
                return true;
            }
            else {
                //TODO: insert error message for sensor malfunction 
                return false;
            }
        }

        public static bool distance_data(string[] lines, int start_line, ref List<double> distances)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = start_line; i < lines.Length; ++i)
            {
                sb.Append(lines[i].Substring(0, lines[i].Length - 1));
            }
            return SCIP_Reader.decode_array(sb.ToString(), 3, ref distances);
        }

        public static long decode(string data, int size, int offset = 0)
        {
            long value = 0;

            for (int i = 0; i < size; ++i)
            {
                value <<= 6;
                value |= (long)data[offset + i] - 0x30;
            }

            return value;
        }

        public static bool decode_array(string data, int size, ref List<double> decoded_data)
        {
            for (int pos = 0; pos <= data.Length - size; pos += size)
            {
                decoded_data.Add(decode(data, size, pos));
            }
            return true;
        }
    }
}

