import "./index.scss";
import { loadAnimation, setSpeed } from "lottie-web";

const loadingElement = document.getElementById('loading')!;

loadAnimation({
    container: loadingElement, // the dom element that will contain the animation
    renderer: 'svg',
    loop: true,
    autoplay: true,
    name: 'loading',
    path: './static/cat-loading.json' // the path to the animation json
});

setSpeed(2, 'loading');

function waitForElement(selector: string) {
    return new Promise(resolve => {
        if (document.querySelector(selector)) {
            return resolve(document.querySelector(selector));
        }

        const observer = new MutationObserver(mutations => {
            if (document.querySelector(selector)) {
                resolve(document.querySelector(selector));
                observer.disconnect();
            }
        });

        observer.observe(document.body, {
            childList: true,
            subtree: true
        });
    });
}


export async function loadLottieAnimation(id: string, path: string, name?: string) {
    await waitForElement(`#${id}`);
    loadAnimation({
        container: document.getElementById(id),
        renderer: 'svg',
        loop: true,
        autoplay: true,
        path,
        name,
    })
}
// https://www.meziantou.net/upload-files-with-drag-drop-or-paste-from-clipboard-in-blazor.htm
export function initializeFileDropZone(dropZoneElement: HTMLElement, inputFile: any) {
    // Add a class when the user drags a file over the drop zone
    function onDragHover(e: DragEvent) {
        e.preventDefault();
        dropZoneElement.classList.add("hover");
    }

    function onDragLeave(e: DragEvent) {
        e.preventDefault();
        dropZoneElement.classList.remove("hover");
    }

    // Handle the paste and drop events
    function onDrop(e: DragEvent) {
        e.preventDefault();
        dropZoneElement.classList.remove("hover");

        // Set the files property of the input element and raise the change event
        inputFile.files = e.dataTransfer?.files;
        const event = new Event('change', { bubbles: true });
        inputFile.dispatchEvent(event);
    }

    function onPaste(e: ClipboardEvent) {
        // Set the files property of the input element and raise the change event
        inputFile.files = e.clipboardData?.files;
        const event = new Event('change', { bubbles: true });
        inputFile.dispatchEvent(event);
    }

    // Register all events
    dropZoneElement.addEventListener("dragenter", onDragHover);
    dropZoneElement.addEventListener("dragover", onDragHover);
    dropZoneElement.addEventListener("dragleave", onDragLeave);
    dropZoneElement.addEventListener("drop", onDrop);
    dropZoneElement.addEventListener('paste', onPaste);

    // The returned object allows to unregister the events when the Blazor component is destroyed
    return {
        dispose: () => {
            dropZoneElement.removeEventListener('dragenter', onDragHover);
            dropZoneElement.removeEventListener('dragover', onDragHover);
            dropZoneElement.removeEventListener('dragleave', onDragLeave);
            dropZoneElement.removeEventListener("drop", onDrop);
            dropZoneElement.removeEventListener('paste', onPaste);
        }
    }
}
