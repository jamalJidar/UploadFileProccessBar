let btnUpload = document.getElementById('btnUpload');
let FileUpload = document.getElementById('FileUpload');
let notification = document.getElementById('notification');
let streamPreview = document.getElementById('streamPreview');
let content = document.getElementById('content');
let files = [];
const Extension = [
    { 'ex': 'jpg', element: 'img', index: 0 },
    { 'ex': 'png', element: 'img', index: 0 },
    { 'ex': 'webp', element: 'img', index: 0 },
    { 'ex': 'gif', element: 'img', index: 0 },
    { 'ex': 'tif', element: 'img', index: 0 },
    { 'ex': 'bmp', element: 'img', index: 0 },
    { 'ex': 'eps', element: 'img', index: 0 },

    { 'ex': 'mp3', element: 'audio', index: 1 },
    { 'ex': 'wma', element: 'audio', index: 1 },
    { 'ex': 'snd', element: 'audio', index: 1 },
    { 'ex': 'wav', element: 'audio', index: 1 },
    { 'ex': 'ra', element: 'audio',  index: 1 },
    { 'ex': 'au', element: 'audio',  index: 1 },
    { 'ex': 'aac', element: 'audio', index: 1 },

    { 'ex': 'mp4', element: 'video', index: 2 },
    { 'ex': '3gp', element: 'video', index: 2 },
    { 'ex': 'avi', element: 'video', index: 2 },
    { 'ex': 'mpg', element: 'video', index: 2 },
    { 'ex': 'mov', element: 'video', index: 2 },
    { 'ex': 'wmv', element: 'video', index: 2 },
    //other files ...


];
btnUpload.addEventListener('click', async function () {


    await GetExtension();
})
async function GetExtension() {
    for await (const item of getFiles()) {
        var medaiName = makeid(10);
        var proccessBarId = makeid(10);
        var div = document.createElement('div');
        var span = document.createElement('span');
        console.log(item);
        var elementGenerator = new ElementGenerator(medaiName, item.name.split('.').pop());
        var medai = elementGenerator.GetElement    //document.createElement('img');
        var h2 = document.createElement('h2');
        var caption = makeid(10);
        h2.setAttribute('id', caption);
        h2.innerHTML = formatBytes(item.size);

        span.setAttribute('id', proccessBarId);

        span.innerHTML = '0%';
       /* medai.setAttribute('id', medaiName);*/
        if (!streamPreview.checked) {
            medai.src = URL.createObjectURL(item)
        }


        div.appendChild(span);
        div.appendChild(medai);
        div.appendChild(h2);
        content.appendChild(div);

        await ConvertFileToByte(item, 0, 0, medaiName, proccessBarId, caption)

    }
}

async function* getFiles() {

    for (let index = 0; index < FileUpload.files.length; index++) {

        yield FileUpload.files[index]
    }
}
class ElementGenerator {
    constructor(_name, _type) {
        this.name = _name.trim()//.toLowerCase();
        this.type = _type.trim()//.toLowerCase;

    }

    get GetElement() {
        return this.Generate();
    }
    Generate() {
        var el = this.check();
        var x = document.createElement(this.check(el));
        x.setAttribute("width", "320");
        x.setAttribute("height", "240");
        x.setAttribute('id', this.name)
        if (el === 'video' || el==='audio')
            x.setAttribute("controls", "controls");

        document.body.appendChild(x);
        return x;
    }

    check() {

        var el = Extension.filter((x) => {
            return x.ex === this.type
        });

        if (el.length < 1) {
            el = 'iframe'
            return el
        }
        return el[0].element;
    }




}
async function postData(item = {}) {

    return new Promise((resolve) => {
        resolve(fetch("/home/Upload", {
            method: "POST", // *GET, POST, PUT, DELETE, etc.
            mode: "cors", // no-cors, *cors, same-origin
            cache: "no-cache", // *default, no-cache, reload, force-cache, only-if-cached
            credentials: "same-origin", // include, *same-origin, omit
            headers: {
                'content-type': "application/octet-stream",
                'persent': Math.round(item.persent),
                'totalByte': item.size,
                'id': item.id,

                // 'Content-Type': 'application/x-www-form-urlencoded',
            },
            //  redirect: "follow", // manual, *follow, error
            referrerPolicy: "no-referrer", // no-referrer, *no-referrer-when-downgrade, origin, origin-when-cross-origin, same-origin, strict-origin, strict-origin-when-cross-origin, unsafe-url
            body: item.file, // body data type must match "Content-Type" header
        }).then((respnsive) => respnsive.json())
            .then((result) => {

                if (result.status === 'ok') {
                    document.getElementById(`percentUpload${id}`).innerHTML = '100%';
                    console.log('success upload', `img${id}`)
                    document.getElementById().setAttribute('src', `data:image/png;base64,${result.file}`);
                    return resolve("ok")
                }

                else //(parseInt(result.index[0]) === index) 
                {
                    document.getElementById(`percentUpload${id}`).innerHTML = result.persent + '%';
                    return resolve("pendding")
                    //return result
                }

                //else {
                //    document.getElementById(`percentUpload${id}`).innerHTML = 'loding';
                //    return resolve("loding")
                //}

            }));

    })

}
async function ConvertFileToByte(file, id, index, medaiName, proccessBarId, caption) {

    var fileDetali = [];

    const fileReader = new FileReader();

    fileReader.readAsArrayBuffer(file);


    fileReader.onload = async (event) => {


        const totalByte = event.target.result.byteLength;
        const content = event.target.result;
        const CHUNK_SIZE = 184935 ;
        const totalChunks = totalByte / CHUNK_SIZE;



        var index = 0
        var id = '';
        var lenUpload = 0;
        for (let chunk = 0; chunk < totalChunks + 1; chunk++) {
            index++;
            let CHUNK = content.slice(chunk * CHUNK_SIZE, (chunk + 1) * CHUNK_SIZE)
            var persent = ((((chunk < 1 ? 1 : chunk) * CHUNK_SIZE) / totalByte) * 100)

            var item = { 'id': id, 'size': totalByte, persent, 'file': CHUNK };
            lenUpload += CHUNK.byteLength;

            document.getElementById(caption).innerHTML = formatBytes(totalByte)
                + '       ' + formatBytes(lenUpload);
            var result = await SaveToDatabase(item, medaiName, proccessBarId ,file. type);
            //   console.log( 'id : ', id , 'resault : ' , result.id)
            id = result.id;

        }


    }


}

function SaveToDatabase(item, medaiName, proccessBarId , type) {
    return new Promise((resolve) => {
        fetch("/home/Upload?type="+type, {
            method: "POST", // *GET, POST, PUT, DELETE, etc.
            mode: "cors", // no-cors, *cors, same-origin
            cache: "no-cache", // *default, no-cache, reload, force-cache, only-if-cached
            credentials: "same-origin", // include, *same-origin, omit
            headers: {
                'content-type': "application/octet-stream",
                'persent': Math.round(item.persent),
                'totalByte': item.size,
                'id': item.id,
                'streamPreview': streamPreview.checked

                // 'Content-Type': 'application/x-www-form-urlencoded',
            },
            //  redirect: "follow", // manual, *follow, error
            referrerPolicy: "no-referrer", // no-referrer, *no-referrer-when-downgrade, origin, origin-when-cross-origin, same-origin, strict-origin, strict-origin-when-cross-origin, unsafe-url
            body: item.file, // body data type must match "Content-Type" header
        }).then((respnsive) => respnsive.json())
            .then((result) => {
                id = result.id

                if (streamPreview.checked) {
                    document.getElementById(medaiName).src = result.file
                }
                if (result.status === 'ok') {
                    document.getElementById(proccessBarId).innerHTML = '100%';

                    if (notification.checked) {
                        const audioElement = new Audio("/notification/notification-tones.mp3");
                        audioElement.play();
                    }
                }

                else {
                    document.getElementById(proccessBarId).innerHTML = result.persent + '%';

                }

                resolve(result)



            });

    })
}


function makeid(length) {
    let result = '';
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    const charactersLength = characters.length;
    let counter = 0;
    while (counter < length) {
        result += characters.charAt(Math.floor(Math.random() * charactersLength));
        counter += 1;
    }
    return result;
}


function formatBytes(bytes, decimals = 2) {
    if (!+bytes) return '0 Bytes'

    const k = 1024
    const dm = decimals < 0 ? 0 : decimals
    const sizes = ['Bytes', 'KiB', 'MiB', 'GiB', 'TiB', 'PiB', 'EiB', 'ZiB', 'YiB']

    const i = Math.floor(Math.log(bytes) / Math.log(k))

    return `${parseFloat((bytes / Math.pow(k, i)).toFixed(dm))} ${sizes[i]}`
}