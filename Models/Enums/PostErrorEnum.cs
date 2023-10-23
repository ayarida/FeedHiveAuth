namespace FeedHiveAuth.Models.Enums
{
    public enum PostErrorEnum
    {
        NO_ERROR = 0,
        MISSING_DATA_TITLE = 1,
        MISSING_DATA_CREATED_BY = 2,
        AREA_NOT_AUTHORIZED = 3,
        INVALID_PARAMETERS = 4,
        NO_TEMPLATE = 5,
        WIDGET_NOY_FOUND = 6,
        NO_RESULTS = 7,
        PostTypeNotFound,
        PostNotFound,
        TermNotFound,
        RelatedToNotFound,

        POST_NOT_VALID,
        USER_UNAUTHORIZED,
        POST_OUTDATED,
        LONG_SHORT_TITLE,
        MISSING_RELATIVE,
        MIN_RELATIONS_NOT_REACHED,
        MAX_RELATIONS_EXCEEDED,
        MISSING_META_DATA,
        MISSING_MEDIA,
        MAX_TERMS_EXCEEDED,
        MISSING_DATA_TITLE_TERMS,
        POST_ALREADY_PUBLISHED,
        POST_ALREADY_UNPUBLISHED,
        POST_UNPUBLISHED,
        POST_ALREADY_DELETED,
        AUTOMATION_IN_PROGRESS,
        OperationFailed,
        STATUS_ALREADY_SET
    }
}
