using Interfaces;
using System;

namespace FileManager
{
    /// <summary>
    /// <para>
    /// Represents the help trigger.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="ITrigger{Context}"/>
    public class HelpTrigger : ITrigger<Context>
    {
        /// <summary>
        /// <para>
        /// Determines whether this instance condition.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="arguments">
        /// <para>The arguments.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The bool</para>
        /// <para></para>
        /// </returns>
        public bool Condition(Context arguments) => arguments.Args[0].ToLower() == "help";

        /// <summary>
        /// <para>
        /// Actions the arguments.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="arguments">
        /// <para>The arguments.</para>
        /// <para></para>
        /// </param>
        public void Action(Context arguments) => Console.WriteLine(@"Use this program to manage links in your links repository. For close just press CTRL+C. 
Avalible commands:
1. Delete [address]
2. Create [address] [path to file]
3. Help
4. Print 
5. Show [file number]
6. CreateFileSet [File set name] {[Path to file in remote storage] [path to file in local storage]}
7. GetFilesByFilesSetName [File set name]");
    }
}
