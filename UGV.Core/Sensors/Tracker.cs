using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGV.Core.Maths;

namespace UGV.Core.Sensors
{
    /* A Tracking class that keeps track of the position. It records the turns and
     * the path taken. */
    public class Tracker
    {

        private LinkedList<double[]> raw_data = new LinkedList<double[]>();
        private LinkedList<double[]> raw_byte_data = new LinkedList<double[]>();
        private LinkedList<Matrix> absolute_path = new LinkedList<Matrix>();
        private double current_direction, previous_direction;
        Matrix current_coord;
        double total_turn = 0;

        private Matrix mod;
        bool initialized = false;

        
        public Tracker(double lat, double lon, double new_direction) 
        {
            double[,] modifier = new double[2, 2] { { 1 / 110923.4638220902, 0 }, { 0, 92318.84550536113 } };
            mod = new Matrix(modifier);
            current_coord = new Matrix(new double[2] { lat, lon });
            current_direction = previous_direction = new_direction;
        }

        public void add(double distance, double turn, double left, double right)
        {
            double[] raw = { distance, turn };
            double[] path;
            double vector_length;

            double new_direction = current_direction + turn / 2;
            total_turn += turn / 2;

            double mean_direction = Utility.AverageAngle(current_direction, previous_direction);

            vector_length = distance / turn * Math.Sin(turn);

            double[] raw_byte = { left, right };
            raw_byte_data.AddLast(raw_byte);

            //set the array up as a vector that map the path as seen from the vehicles perspective before it moved
            //<forward-back, left-right> right is negative, left is positive

            path = new double[] { vector_length, vector_length };
            path[0] = Math.Cos(turn / 2) * path[0];
            path[1] = Math.Sin(turn / 2) * path[1];

            //rotates vector to a north south orientation <east-west, north-south>
            path[1] = path[0] * Math.Cos(mean_direction) + path[1] * Math.Sin(mean_direction);
            path[0] = path[0] * Math.Sin(mean_direction) - path[1] * Math.Cos(mean_direction);

            raw_data.AddLast(raw);
            absolute_path.AddLast(new Matrix(path));
            Matrix update_coord = new Matrix(path);
            update_coord = update_coord * mod;
            current_coord += update_coord;
        }

        
        public Matrix totalPath()
        {
            Matrix total_path = new Matrix(new double[] { 0, 0 });
            LinkedListNode<Matrix> current_vector = absolute_path.First;

            while (current_vector.Next != null)
            {
                total_path = total_path - current_vector.Value;
                current_vector = current_vector.Next;
            }

            return total_path;
        }

        public Matrix getMostRecent()
        {
            return absolute_path.Last.Value;
        }

        public double getLatitude() 
        {
            return current_coord.Get(0, 1);
        }

        public double getLongitude()
        {
            return current_coord.Get(0, 0);
        }

        double get_total_turn()
        {
            return total_turn;
        }
    }
}