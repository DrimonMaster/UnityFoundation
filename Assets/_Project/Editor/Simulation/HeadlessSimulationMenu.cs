#if HEADLESS_SIM
using UnityEditor;
using UnityFoundation.Simulation;

namespace UnityFoundation.Editor
{
    public static class HeadlessSimulationMenu
    {
        [MenuItem("Tools/UnityFoundation/Run Headless Simulation")]
        public static void RunHeadlessSimulation() => HeadlessSimulation.RunSimulation();
    }
}
#endif
