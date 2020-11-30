using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Charter3HourLogin.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Entry), typeof(CustomEntryRenderer))]
namespace Charter3HourLogin.Droid.Renderers
{
    public class CustomEntryRenderer : EntryRenderer
    {
        public CustomEntryRenderer(Context context) : base(context)
        {
        }
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
        
            if (e.OldElement == null)
            {
                var nativeEditText = (global::Android.Widget.EditText)Control;
                var shape = new ShapeDrawable(new Android.Graphics.Drawables.Shapes.RectShape());
                if (shape.Paint != null)
                {
                    shape.Paint.Color = Xamarin.Forms.Color.LightGray.ToAndroid();
                    shape.Paint.SetStyle(Paint.Style.Stroke);
                }

                nativeEditText.Background = shape;
                
                
            }
        }
    }
}