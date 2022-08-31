import { useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { toast, ToastContainer } from "react-toastify";
import { Badge, Collapse, Container, DropdownItem, DropdownMenu, DropdownToggle, Nav, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink, UncontrolledDropdown } from "reactstrap";
import { useAllServerNames, useNumberOfActivePlayers, useSelectedGameServerName } from "../hooks/gameServersHooks";
import ServerClient from "../libs/ServerClient";
import { routeToServer } from "../routes/routesList";
import 'react-toastify/dist/ReactToastify.css';

type Props = {
    children?: React.ReactNode;
};

export function Layout({ children }: Props) {
    const [isOpen, setIsOpen] = useState(true);
    const toggle = () => setIsOpen(!isOpen);
    const selectedServer = useSelectedGameServerName()
    const allServerNames = useAllServerNames()
    const navigate = useNavigate();
    const params = useParams()
    const serverId = params.serverId

    return (
        <div>
            <ToastContainer position="bottom-center" newestOnTop={true} pauseOnFocusLoss={true} />
            <Navbar>
                <NavbarBrand>
                    <UncontrolledDropdown>
                        <DropdownToggle nav caret>
                            Bedrock server {serverId}
                        </DropdownToggle>
                        <DropdownMenu end>
                            {allServerNames?.map(serverName => (
                                <DropdownItem active={serverId === serverName} key={serverName} tag="button" action="true" className="floatRight" >
                                    <ServerButton serverName={serverName} />
                                </DropdownItem>
                            ))}
                        </DropdownMenu>
                    </UncontrolledDropdown>
                </NavbarBrand>
                <NavbarToggler onClick={toggle} />
                <Collapse isOpen={isOpen} navbar>
                    <Nav pills>
                        <NavItem>
                            <NavLink href="#" onClick={() => { navigate(routeToServer(selectedServer)) }}>
                                all
                            </NavLink>
                        </NavItem>
                        <NavItem>
                            <NavLink href="#" onClick={() => { navigate(routeToServer(selectedServer) + "/console") }}>
                                Console logs
                            </NavLink>
                        </NavItem>
                        <NavItem>
                            <NavLink href="#" onClick={() => { navigate(routeToServer(selectedServer) + "/users") }}>
                                Users
                            </NavLink>
                        </NavItem>
                        <NavItem>
                            <NavLink href="#" onClick={() => { navigate(routeToServer(selectedServer) + "/backups") }}>
                                Backups
                            </NavLink>
                        </NavItem>
                        <UncontrolledDropdown nav inNavbar>
                            <DropdownToggle nav caret>
                                Actions
                            </DropdownToggle>
                            <DropdownMenu end>
                                <DropdownItem><ServerBackupButton /></DropdownItem>
                                <DropdownItem><ShowToastButton /></DropdownItem>
                            </DropdownMenu>
                        </UncontrolledDropdown>
                    </Nav>
                </Collapse>
            </Navbar>
            <div>
                <div className="App">
                    <Container fluid="true">
                        {children}
                    </Container>
                </div>
            </div>
        </div>
    )
}

function ShowToastButton() {
    const showToast = () => toast.info('ala')
    return (
        <div onClick={showToast}>toast</div>
    )
}

function ServerBackupButton() {
    const serverId = useSelectedGameServerName()
    function doBackup() {
        ServerClient.DoBackup(serverId)
    }

    return (
        <div onClick={doBackup}>Backup
        </div>
    )
}

interface ServerButtonParams {
    serverName: string
}

function ServerButton(props: ServerButtonParams) {
    const activeUsersCount = useNumberOfActivePlayers(props.serverName)
    const navigate = useNavigate();

    const setParam = () => {
        navigate(routeToServer(props.serverName))
        //window.location.href = window.location.href
    }


    return (
        <div onClick={setParam} className="text-break">
            {props.serverName}
            <Badge pill>{activeUsersCount}</Badge>
        </div>
    );
}
