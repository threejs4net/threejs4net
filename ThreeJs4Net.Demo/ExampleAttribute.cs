#region --- License ---
/* Copyright (C) 2006-2008 the OpenTK team
 * See license.txt for licensing details
 */
#endregion

using System;

namespace ThreeJs4Net.Demo
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExampleAttribute : System.Attribute
    {
        public string Title { get; internal set; }

        public readonly ExampleCategory Category;

        public readonly string Subcategory;

        public float LevelComplete;

        public string Documentation;

        public ExampleAttribute(string title, ExampleCategory category, string subcategory, float levelComplete = 1)
        {
            this.Title = title;
            this.Category = category;
            this.Subcategory = subcategory;
            this.LevelComplete = levelComplete;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0}: {1}", Category, Title);
        }
    }

    public enum ExampleCategory
    {
        OpenTK = 0,
        Misc
    }
}
