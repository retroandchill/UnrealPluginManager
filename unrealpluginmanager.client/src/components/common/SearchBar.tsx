import {SearchIconWrapper} from "./SearchIconWrapper";
import {StyledInputBase} from "./StyledInputBase";
import {Search} from "./Search";
import SearchIcon from '@mui/icons-material/Search';
import {HTMLAttributes, KeyboardEvent, useState} from "react";
import {SxProps, Theme} from "@mui/material";

interface SearchBarProps extends HTMLAttributes<HTMLDivElement> {
  onSearch?: (search: string) => void;
  allowEmptySearch?: boolean;
  placeholder?: string;
  sx?: SxProps<Theme>;
}

export function SearchBar({
                            onSearch,
                            allowEmptySearch = false,
                            placeholder = 'Search...',
                            ...boxProps
                          }: Readonly<SearchBarProps>) {
  const [search, setSearch] = useState('');

  function handleKeyDown(event: KeyboardEvent<HTMLInputElement>) {
    if (event.key === 'Enter' && (search.length > 0 || allowEmptySearch)) {
      onSearch?.(search);
    }
  }


  return (
      <Search {...boxProps}>
        <SearchIconWrapper>
          <SearchIcon/>
        </SearchIconWrapper>
        <StyledInputBase
            placeholder={placeholder}
            inputProps={{'aria-label': 'search'}}
            onChange={e => setSearch(e.target.value)}
            value={search}
            onKeyDown={handleKeyDown}
        />
      </Search>
  );
}