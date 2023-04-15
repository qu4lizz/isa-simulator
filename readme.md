# Simulator for Instruction Set Architecture

## Description

<p align="justify">This project is a simulator for an Instruction Set Architecture with 4 general-purpose registers of 64 bits each. The simulator functions as an interpreter. It allows loading of assembler code from a file and performs proper lexical, syntactical, and semantical analysis of the code. The instruction set for the simulated machine includes: </p>

- Basic arithmetic operations (ADD, SUB, MUL, DIV)
- Basic bitwise logical operations (AND, OR, NOT, XOR)
- Instruction to move data between registers (MOV)
- Instruction to input data from standard input
- Instruction to output data to standard output

<p align="justify">The simulator also includes single-step debugging support that allows for executing and viewing the values of all registers and memory addresses specified as breakpoints in the assembler code during execution. This mode allows jumping to the next instruction (NEXT command) and to the next breakpoint (CONTINUE).</p>

<p align="justify">The simulator allows working with a 64-bit address space for the simulated machine's memory. It allows for direct and indirect addressing, and the content of each memory address is 1 byte in length. It also enables accessing all addresses in the address space, including read and write operations, using the appropriate instructions (MOV or LOAD/STORE). </p>

<p align="justify">The simulator implements instructions necessary for unconditional and conditional branching (JMP, CMP, JE, JNE, JGE, JL). Instead of interpreting strings directly, the simulator allows assembler instructions to be translated into machine code (bytecode) and stored in the simulated machine's address space (analogous to the code or text segment). The instructions are then interpreted and executed from the simulated machine's memory. A program counter register is defined and used as a pointer to the current instruction.</p>