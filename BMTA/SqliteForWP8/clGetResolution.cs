using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace BMTA
{
    class clGetResolution
    {
        static bool _initialized = false;
        static int _height;
        static int _width;

        public static int Height
        {
            get
            {
                if (!_initialized)
                    ReadValues();
                return _height;
            }
        }
        public static int Width
        {
            get
            {
                if (!_initialized)
                    ReadValues();
                return _width;
            }
        }

        static void ReadValues()
        {
            _initialized = true;
            _height = 800;
            _width = 480;
            if (Environment.OSVersion.Version.Major < 8)
                return;
            int scaleFactor = (int)GetProperty(Application.Current.Host.Content, "ScaleFactor");
            switch (scaleFactor)
            {
                case 100:
                    //same resolution
                    //nothing to do
                    break;
                case 150:
                    _width = 720;
                    _height = 1280;
                    break;
                case 160:
                    _width = 768;
                    _height = 1280;
                    break;
            }
        }

        private static object GetProperty(object instance, string name)
        {
            var getMethod = instance.GetType().GetProperty(name).GetGetMethod();
            return getMethod.Invoke(instance, null);
        }
    }
}
