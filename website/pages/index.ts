import { send } from "../utilities";

type Group = {
    Id: number,
    Name: string,
};

type Student = {
    Id: number,
    Name: string,
}

type Subject = {
    Id: number,
    Name: string,
    Teacher: string,
}

type Grade = {
    Score: number,
    Subject: Subject,
}

let groupSelect = document.querySelector("#groupSelect") as HTMLSelectElement;
let studentSelect = document.querySelector("#studentSelect") as HTMLSelectElement;

async function updateStudentSelect() {
    let groupId = parseInt(groupSelect.value);
    let students = await send("getStudents", groupId) as Student[];
    studentSelect.innerHTML = "";
    for (let student of students) {
        let option = document.createElement("option");
        option.value = student.Id.toString();
        option.innerText = student.Name;
        studentSelect.appendChild(option);
    }
};


let groups = await send("getGroups", []) as Group[];

for (let group of groups) {
    let option = document.createElement("option");
    option.value = group.Id.toString();
    option.innerText = group.Name;
    groupSelect.appendChild(option);
}

await updateStudentSelect();

groupSelect.onchange = async function () {
    await updateStudentSelect();
}