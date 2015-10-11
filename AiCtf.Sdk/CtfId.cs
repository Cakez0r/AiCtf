namespace AiCtf.Sdk
{
    /// <summary>
    /// Generates unique ids for game entities
    /// </summary>
    public static class CtfId
    {
        private static int s_id = 0;
        private static object m_lock = new object();

        /// <summary>
        /// Generate a unique id
        /// </summary>
        public static int NewId()
        {
            lock (m_lock)
            {
                return s_id++;
            }
        }
    }
}