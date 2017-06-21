using System;
using System.IO;
using System.Text;

namespace Poloniex.LIB.Configuration
{
    /// <summary>
    /// 로그처리와 관련된 Server Library Class
    /// </summary>
    public class CLogger
    {
        //-------------------------------------------------------------------------------------------------------------------------
        //
        //-------------------------------------------------------------------------------------------------------------------------
        private static CConfig __cconfig = new CConfig();

        /// <summary>
        /// 
        /// </summary>
        public CLogger(string p_log_name = "BTC-TRADING")
        {
            this.LogFileName = p_log_name;
        }

        /// <summary>
        /// 
        /// </summary>
        public string LogFileName
        {
            get;
            set;
        }

        //-------------------------------------------------------------------------------------------------------------------------
        //
        //-------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_message">메시지</param>
        public void WriteLog(string p_message)
        {
            WriteLog("I", p_message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_exception">예외(L,X,I,etc)</param>
        public void WriteLog(Exception p_exception)
        {
            if (p_exception.InnerException == null)
                WriteLog("X", p_exception.ToString());
            else
                WriteLog(p_exception.InnerException);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_message_type">예외(L,X,I,etc)</param>
        /// <param name="p_message">메시지</param>
        public void WriteLog(string p_message_type, string p_message)
        {
            if (CRegistry.UserInteractive == true)
            {
                WriteDebug(p_message_type, p_message);

                if (p_message_type == "X")
                    WriteFile(p_message_type, p_message);
            }
            else
                WriteFile(p_message_type, p_message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_message_type"></param>
        /// <param name="p_message"></param>
        private void WriteFile(string p_message_type, string p_message)
        {
            var _fs = (FileStream)null;

            try
            {
                var _log_time = DateTime.Now;

                var _log_directory = __cconfig.GetLoggingFolder(String.Format("{0:yyyyMM}", _log_time));
                var _log_file = String.Format("{0}{1:yyyyMMdd}.log", this.LogFileName, _log_time);
                var _log_path = Path.Combine(_log_directory, _log_file);

                _fs = new FileStream(_log_path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                using (var _sw = new StreamWriter(_fs, Encoding.UTF8))
                {
                    _sw.WriteLine(String.Format("[{0}] {1}, {2}", _log_time.ToLogDateTimeString(), p_message_type, p_message));
                    _sw.Flush();
                }
            }
            catch (Exception ex)
            {
                WriteDebug(ex, p_message);
            }
            finally
            {
                if (_fs != null)
                    _fs.Dispose();
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------
        //
        //-------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_message">메시지</param>
        public void WriteDebug(string p_message)
        {
            WriteDebug("I", p_message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_exception">예외(L,X,I,etc)</param>
        public void WriteDebug(Exception p_exception)
        {
            WriteDebug("X", p_exception.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_exception">예외(L,X,I,etc)</param>
        /// <param name="p_message">메시지</param>
        public void WriteDebug(Exception p_exception, string p_message)
        {
            WriteDebug("X", String.Format("'{0}', {1}", p_message, p_exception.Message));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_message_type">예외(L,X,I,etc)</param>
        /// <param name="p_message">메시지</param>
        private void WriteDebug(string p_message_type, string p_message)
        {
            if (CRegistry.UserInteractive == true)
            {
                var _logmsg = String.Format("[{0:yyyy-MM-dd-HH:mm:ss}] {1}, {2}", DateTime.Now, p_message_type, p_message);
                Console.WriteLine(_logmsg);
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------
        //
        //-------------------------------------------------------------------------------------------------------------------------
    }
}