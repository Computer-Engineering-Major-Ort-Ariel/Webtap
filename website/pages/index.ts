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
let gradesTable = document.querySelector("#gradesTable") as HTMLTableElement;


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

async function updateGradesTable() {
    let studentId = parseInt(studentSelect.value);
    let grades = await send("getGrades", studentId) as Grade[];
    console.log("grades");
    gradesTable.innerHTML = "";
    // for (let grade of grades) {
    //     let tr = document.createElement("tr");
    //     gradesTable.appendChild(tr);

    //     let subjectTd = document.createElement("td");
    //     tr.appendChild(subjectTd);

    //     let subjectNameDiv = document.createElement("div");
    //     subjectNameDiv.innerText = grade.Subject.Name;
    //     subjectTd.appendChild(subjectNameDiv);

    //     let subjectTeacherDiv = document.createElement("div");
    //     subjectTeacherDiv.innerText = grade.Subject.Teacher;
    //     subjectTd.appendChild(subjectTeacherDiv);

    //     let ScoreTd = document.createElement("td");
    //     ScoreTd.innerText = grade.Score.toString();
    //     tr.appendChild(ScoreTd);
    // }
}


let groups = await send("getGroups", []) as Group[];

for (let group of groups) {
    let option = document.createElement("option");
    option.value = group.Id.toString();
    option.innerText = group.Name;
    groupSelect.appendChild(option);
}

await updateStudentSelect();

groupSelect.onchange = async function() {
    await updateStudentSelect();
    await updateGradesTable();
}

await updateGradesTable();

studentSelect.onchange = updateGradesTable;
