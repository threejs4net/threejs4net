using ThreeJs4Net.Loaders;
using ThreeJs4Net.Textures;

namespace ThreeJs4Net
{
    public class Three
    {
        public static int LineStrip = 0;
        public static int LinePieces = 1;

        // GL STATE CONSTANTS

        public static int CullFaceNone = 0;
        public static int CullFaceBack = 1;
        public static int CullFaceFront = 2;
        public static int CullFaceFrontBack = 3;

        public static int FrontFaceDirectionCW = 0;
        public static int FrontFaceDirectionCCW = 1;

        // SHADOWING TYPES

        public static int BasicShadowMap = 0;
        public static int PCFShadowMap = 1;
        public static int PCFSoftShadowMap = 2;

        // MATERIAL CONSTANTS

        // side

        public static int FrontSide = 0;
        public static int BackSide = 1;
        public static int DoubleSide = 2;

        // shading

        public static int NoShading = 0;
        public static int FlatShading = 1;
        public static int SmoothShading = 2;

        // colors

        public static int NoColors = 0;
        public static int FaceColors = 1;
        public static int VertexColors = 2;

        // blending modes

        public static int NoBlending = 0;
        public static int NormalBlending = 1;
        public static int AdditiveBlending = 2;
        public static int SubtractiveBlending = 3;
        public static int MultiplyBlending = 4;
        public static int CustomBlending = 5;

        // custom blending equations
        // (numbers start from 100 not to clash with other
        //  mappings to OpenGL constants defined in Texture.js)

        public static int AddEquation = 100;
        public static int SubtractEquation = 101;
        public static int ReverseSubtractEquation = 102;

        // custom blending destination factors

        public static int ZeroFactor = 200;
        public static int OneFactor = 201;
        public static int SrcColorFactor = 202;
        public static int OneMinusSrcColorFactor = 203;
        public static int SrcAlphaFactor = 204;
        public static int OneMinusSrcAlphaFactor = 205;
        public static int DstAlphaFactor = 206;
        public static int OneMinusDstAlphaFactor = 207;

        // custom blending source factors

        //public static int ZeroFactor = 200;
        //public static int OneFactor = 201;
        //public static int SrcAlphaFactor = 204;
        //public static int OneMinusSrcAlphaFactor = 205;
        //public static int DstAlphaFactor = 206;
        //public static int OneMinusDstAlphaFactor = 207;
        public static int DstColorFactor = 208;
        public static int OneMinusDstColorFactor = 209;
        public static int SrcAlphaSaturateFactor = 210;


        // TEXTURE CONSTANTS

        public static int MultiplyOperation = 0;
        public static int MixOperation = 1;
        public static int AddOperation = 2;

        // Mapping modes


        public class UVMapping : TextureMapping { }

        public class CubeReflectionMapping : TextureMapping { }
        public class CubeRefractionMapping : TextureMapping { }

        public class SphericalReflectionMapping : TextureMapping { }
        public class SphericalRefractionMapping : TextureMapping { }

        // Wrapping modes

        public static int RepeatWrapping = 1000;
        public static int ClampToEdgeWrapping = 1001;
        public static int MirroredRepeatWrapping = 1002;

        // Filters

        public static int NearestFilter = 1003;
        public static int NearestMipMapNearestFilter = 1004;
        public static int NearestMipMapLinearFilter = 1005;
        public static int LinearFilter = 1006;
        public static int LinearMipMapNearestFilter = 1007;
        public static int LinearMipMapLinearFilter = 1008;

        // Data types

        public static int UnsignedByteType = 1009;
        public static int ByteType = 1010;
        public static int ShortType = 1011;
        public static int UnsignedShortType = 1012;
        public static int IntType = 1013;
        public static int UnsignedIntType = 1014;
        public static int FloatType = 1015;

        // Pixel types

        //public static int UnsignedByteType = 1009;
        public static int UnsignedShort4444Type = 1016;
        public static int UnsignedShort5551Type = 1017;
        public static int UnsignedShort565Type = 1018;

        // Pixel formats

        public static int AlphaFormat = 1019;
        public static int RGBFormat = 1020;
        public static int RGBAFormat = 1021;
        public static int LuminanceFormat = 1022;
        public static int LuminanceAlphaFormat = 1023;

        public static int BGRFormat = 1030;
        public static int BGRAFormat = 1031;

        // Compressed texture formats

        public static int RGB_S3TC_DXT1_Format = 2001;
        public static int RGBA_S3TC_DXT1_Format = 2002;
        public static int RGBA_S3TC_DXT3_Format = 2003;
        public static int RGBA_S3TC_DXT5_Format = 2004;

        /*
        // Potential future PVRTC compressed texture formats
        public static int RGB_PVRTC_4BPPV1_Format = 2100;
        public static int RGB_PVRTC_2BPPV1_Format = 2101;
        public static int RGBA_PVRTC_4BPPV1_Format = 2102;
        public static int RGBA_PVRTC_2BPPV1_Format = 2103;
        */

        public static LoadingManager DefaultLoadingManager = null;
    }
}