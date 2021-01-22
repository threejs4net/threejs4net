using ThreeJs4Net.Cameras;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Extras.Renderers
{
    public interface IPlugin
    {
        void init();

        void render(Scene scene, Camera camera, int viewportWidth, int viewportHeight );
    }
}
