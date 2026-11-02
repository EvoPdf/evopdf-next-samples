converter.PrepareRenderPdfPageEvent += (eventParams) =>
{
    if (eventParams.PageNumber == 1)
        eventParams.Page.ShowHeader = false;
};
