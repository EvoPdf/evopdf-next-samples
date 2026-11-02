htmlToPdfConverter.ScriptToExecuteAfterLoad =
    "await new Promise(resolve => setTimeout(resolve, 2000));" +
    "document.body.insertAdjacentHTML('beforeend', '<p>Added by the script executed after load</p>');";
htmlToPdfConverter.WaitForAfterLoadScript = true;
