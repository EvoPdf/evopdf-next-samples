FROM public.ecr.aws/lambda/dotnet:10

# Install the packages required by the HTML to PDF Converter and the DejaVu font families
RUN dnf install -y nss at-spi2-atk cairo pango dejavu-sans-fonts dejavu-serif-fonts dejavu-sans-mono-fonts && dnf clean all

# Set DejaVu Sans as the sans-serif font
RUN printf '<fontconfig><alias><family>sans-serif</family><prefer><family>DejaVu Sans</family></prefer></alias></fontconfig>' > /etc/fonts/local.conf

# Copy the published function
COPY publish/ ${LAMBDA_TASK_ROOT}

# Ensure execute permissions for the HTML to PDF Converter runtime
RUN chmod +x ${LAMBDA_TASK_ROOT}/evopdf_runtimes/linux-x64/native/evopdf_loadhtml

# The only writable folder of a Lambda function
ENV HOME=/tmp

CMD ["PdfFunction::PdfFunction.Function::FunctionHandler"]
