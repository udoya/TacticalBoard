import r6operators from "r6operators";
import { exec } from "child_process";
import { promisify } from "util";

const execPromise = promisify(exec);

const icons_path = "./icons/png";
const output_path = "./output";

const atk_folder = `${output_path}/atk_operators`;
const def_folder = `${output_path}/def_operators`;

async function createDirectory(path) {
    try {
        const { stdout, stderr } = await execPromise(
            `powershell -Command "if (-Not (Test-Path '${path}')) { New-Item -Path '${path}' -ItemType Directory }"`
        );
        if (stderr) {
            console.error(`cannot create directory: ${stderr}`);
        } else {
            console.log(`created directory: ${path}`);
        }
    } catch (error) {
        console.error(`Error: ${error.message}`);
    }
}

async function copyFile(src, dest) {
    try {
        const { stdout, stderr } = await execPromise(
            `powershell Copy-Item "${src}" -Destination "${dest}"`
        );
        if (stderr) {
            console.error(`cannot copy file: ${stderr}`);
        } else {
            console.log(`copied file: ${src} to ${dest}`);
        }
    } catch (error) {
        console.error(`Error: ${error.message}`);
    }
}

async function main() {
    await createDirectory(output_path);
    await createDirectory(atk_folder);
    await createDirectory(def_folder);

    const copyPromises = [];
    console.log("created directories");

    for (const operator in r6operators) {
        const op = r6operators[operator];
        const role_name = op.role;
        let folder = null;

        if (role_name == "Attacker" || role_name == "Defender") {
            if (op.role == "Attacker") {
                folder = atk_folder;
            } else {
                folder = def_folder;
            }

            const icon_path_operator = `${icons_path}/${operator}.png`;
            const output_path_operator = `${folder}/${operator}.png`;

            copyPromises.push(
                copyFile(icon_path_operator, output_path_operator)
            );
        }
    }

    console.log("Copying files...(prepared promises)");

    await Promise.all(copyPromises);
}

main();
