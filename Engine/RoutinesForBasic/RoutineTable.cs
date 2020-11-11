using System;
using System.Collections;

namespace Engine
{
    public class RoutineTable
    {
        public RoutineTable() { }

        public void RoutineByEvent(int id) {
            switch (id) {
                case 0:
                    break;
                case 1:
                    break;
                default:
                    throw new Exception("Id could not be matched to known Routine!");
            }
        }
    }
}
