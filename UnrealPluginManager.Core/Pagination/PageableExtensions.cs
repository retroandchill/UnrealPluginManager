using LanguageExt;

namespace UnrealPluginManager.Core.Pagination;

public static class PageableExtensions {

    public static Option<PageRequest> ToOption(this Pageable pageable) {
        return pageable;
    }
    
}