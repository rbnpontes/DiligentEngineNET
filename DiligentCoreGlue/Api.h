#ifdef ENGINE_DLL
    #undef ENGINE_DLL
#endif

#if defined(_MSC_VER)
    #define EXPORT_API __declspec(dllexport)
#elif defined(__GNUC__)
    #define EXPORT_API __attribute__((visibility("default")))
#endif

#define EXPORT extern "C" EXPORT_API