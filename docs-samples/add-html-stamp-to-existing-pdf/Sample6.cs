stamp.RenderUnderContent = true;

// The stamp is skipped in the second page
stamp.OnPageRendering = placement => placement.DocumentPageNumber != 2;
